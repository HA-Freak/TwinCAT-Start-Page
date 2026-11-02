using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Linq;

using EnvDTE80;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;

namespace TwinCAT.XAE.StartPage.UI.Services.Other
{
    using Options.Pages;

    using UI.Views;

    public sealed class TwinCatProjectTemplate
    {
        private readonly Lazy<ImageSource> _icon;

        public TwinCatProjectTemplate(string path, string category, int categoryOrder,
            IEnumerable<string> platforms, IEnumerable<string> projectTypes,
            string name = null, string description = null, string baseName = null, string iconPath = null, string subCategory = null)
        {
            var fileName = System.IO.Path.GetFileNameWithoutExtension(path);

            Path = path;
            Category = category;
            SubCategory = subCategory;
            CategoryOrder = categoryOrder;
            ProjectTypes = projectTypes.ToList();
            Tags = platforms.Concat(ProjectTypes).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            Name = string.IsNullOrWhiteSpace(name) ? fileName : name;
            Description = description ?? "";
            BaseName = string.IsNullOrWhiteSpace(baseName) ? fileName : baseName;
            _icon = new Lazy<ImageSource>(() => TwinCatProject.LoadIcon(path, iconPath));
        }

        // the New Project dialog's category, e.g. 'TwinCAT Projects'
        public string Category { get; }

        // the node below the category, e.g. 'Scope' under 'TwinCAT Measurement'; null for none
        public string SubCategory { get; }

        // position of the category in the dialog's category list
        public int CategoryOrder { get; }

        // what the dialog's project type filter offers, e.g. 'Scope'
        public IReadOnlyList<string> ProjectTypes { get; }

        // shown as chips: the platform ('TwinCAT') followed by the project types
        public IReadOnlyList<string> Tags { get; }

        public string Name { get; }

        public string Description { get; }

        // new projects are named <BaseName>1, <BaseName>2, ...
        public string BaseName { get; }

        public string Path { get; }

        public ImageSource Icon
            => _icon.Value;
    }

    // creates TwinCAT XAE projects the way Beckhoff's Automation Interface does:
    // Solution.Create + Solution.AddFromTemplate with TwinCAT's own templates
    internal static class TwinCatProject
    {
        private static readonly Guid TwinCatPackage = new Guid("3d3e7b23-b958-4a99-bfee-d0d953c182d4");

        // the folders TwinCAT registers under NewProjectTemplates\TemplateDirs, in its order, with the
        // package string its New Project dialog shows as the category, that category's position in
        // the dialog (see ExtensionCategoryOrders), and the project type the templates get -
        // unlike .vstman manifests, .vsdir files don't carry type tags
        private static readonly (string Folder, string CategoryResource, string CategoryFallback, int CategoryOrder, string ProjectType)[] TemplateFolders =
        {
            (@"Components\Base\PrjTemplate", "#207", "TwinCAT Projects", TwinCatProjectsCategoryOrder, "XAE"),
            (@"Components\Base\PlcTemplate", "#208", "TwinCAT PLC", 20, "PLC"),
        };

        // the 'TwinCAT Projects' name comes from a package resource and may be localized - find it by position
        public const int TwinCatProjectsCategoryOrder = 10;

        public const string MeasurementCategory = "TwinCAT Measurement";
        public const string HmiCategory = "TwinCAT HMI";
        public const string ConnectivityCategory = "TwinCAT Connectivity";

        // positions of the extension categories, matching the shell's New Project tree:
        // Measurement, HMI, (TwinCAT Projects, TwinCAT PLC), Connectivity, then the rest by name
        private static readonly Dictionary<string, int> ExtensionCategoryOrders = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            [MeasurementCategory] = 0,
            [HmiCategory] = 1,
            [ConnectivityCategory] = 30,
        };

        private const int OtherExtensionCategoryOrder = 40;

        private static readonly string[] TwinCatPlatform = { "TwinCAT" };

        private static readonly string[] TemplateExtensions = { "*.tsproj", "*.tspproj" };

        // TwinCAT functions (Measurement, Connectivity, ...) install as shell extensions whose
        // extension.vsixmanifest declares a ProjectTemplate asset folder with .vstman manifests
        private const string ExtensionsFolder = "Extensions";
        private const string ProjectTemplateAsset = "Microsoft.VisualStudio.ProjectTemplate";
        private const string TwinCatPlatformTag = "TwinCAT";

        private static readonly XNamespace VsixNamespace = "http://schemas.microsoft.com/developer/vsx-schema/2011";
        private static readonly XNamespace ManifestNamespace = "http://schemas.microsoft.com/developer/vstemplatemanifest/2015";
        private static readonly XNamespace TemplateNamespace = "http://schemas.microsoft.com/developer/vstemplate/2005";

        private const string FILE_CLOSE_SOLUTION = "File.CloseSolution";

        // the TwinCAT XAE and PLC templates first (the default template is the first one),
        // then the templates of the installed TwinCAT functions
        public static IReadOnlyList<TwinCatProjectTemplate> FindTemplates()
            => FindFolderTemplates().Concat(FindExtensionTemplates()).ToList();

        public static TwinCatProjectTemplate FindDefaultTemplate()
            => FindFolderTemplates().FirstOrDefault() ?? FindExtensionTemplates().FirstOrDefault();

        // per folder: the templates registered in its .vsdir (in TwinCAT's order), then any other
        // template files placed there, e.g. the user's own
        private static List<TwinCatProjectTemplate> FindFolderTemplates()
        {
            var templates = new List<TwinCatProjectTemplate>();
            var installDir = ReadInstallDir();

            if (string.IsNullOrEmpty(installDir))
                return templates;

            foreach (var (relativeFolder, categoryResource, categoryFallback, categoryOrder, projectType) in TemplateFolders)
            {
                var folder = Path.Combine(installDir, relativeFolder);

                if (!Directory.Exists(folder))
                    continue;

                var category = LoadPackageString(TwinCatPackage, categoryResource) ?? categoryFallback;
                var projectTypes = new[] { projectType };
                var registered = ReadVsDirTemplates(folder, category, categoryOrder, projectTypes);

                var unregistered = TemplateExtensions
                    .SelectMany(pattern => Directory.EnumerateFiles(folder, pattern))
                    .Where(path => !registered.Any(t => string.Equals(t.Path, path, StringComparison.OrdinalIgnoreCase)))
                    .Select(path => new TwinCatProjectTemplate(path, category, categoryOrder, TwinCatPlatform, projectTypes))
                    .OrderBy(t => t.Name, StringComparer.CurrentCultureIgnoreCase);

                templates.AddRange(registered);
                templates.AddRange(unregistered);
            }

            return templates;
        }

        // the same discovery the shell does: every extension in its Extensions folder (Common7\IDE\Extensions)
        // whose extension.vsixmanifest declares a ProjectTemplate asset; only templates tagged with the
        // TwinCAT platform are taken
        private static List<TwinCatProjectTemplate> FindExtensionTemplates()
        {
            var extensionsRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ExtensionsFolder);
            var entries = new List<(int SortOrder, TwinCatProjectTemplate Template)>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (!Directory.Exists(extensionsRoot))
                return new List<TwinCatProjectTemplate>();

            foreach (var vsixManifest in EnumerateFilesSafe(extensionsRoot, "extension.vsixmanifest"))
            {
                try
                {
                    var package = XDocument.Load(vsixManifest).Root;
                    var extensionFolder = Path.GetDirectoryName(vsixManifest);
                    var displayName = (string)package?.Element(VsixNamespace + "Metadata")?.Element(VsixNamespace + "DisplayName");

                    var templateFolders = package?.Element(VsixNamespace + "Assets")?.Elements(VsixNamespace + "Asset")
                        .Where(a => (string)a.Attribute("Type") == ProjectTemplateAsset)
                        .Select(a => Path.Combine(extensionFolder, (string)a.Attribute("Path") ?? ""))
                        .Where(Directory.Exists)
                        ?? Enumerable.Empty<string>();

                    foreach (var templateFolder in templateFolders)
                    {
                        foreach (var templateManifest in Directory.EnumerateFiles(templateFolder, "*.vstman", SearchOption.AllDirectories))
                        {
                            foreach (var entry in ReadTemplateManifest(templateManifest, extensionFolder, displayName))
                            {
                                if (seen.Add(entry.Template.Path))
                                    entries.Add(entry);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    // an unreadable extension manifest only hides that extension's templates
                }
            }

            return entries
                .OrderBy(e => e.SortOrder)
                .ThenBy(e => e.Template.Name, StringComparer.CurrentCultureIgnoreCase)
                .Select(e => e.Template)
                .ToList();
        }

        private static IEnumerable<(int SortOrder, TwinCatProjectTemplate Template)> ReadTemplateManifest(string templateManifest, string extensionFolder, string extensionName)
        {
            var entries = new List<(int SortOrder, TwinCatProjectTemplate Template)>();

            try
            {
                var containers = XDocument.Load(templateManifest).Root
                    .Elements(ManifestNamespace + "VSTemplateContainer")
                    .Where(c => (string)c.Attribute("TemplateType") == "Project");

                foreach (var container in containers)
                {
                    var relativePath = (string)container.Element(ManifestNamespace + "RelativePathOnDisk") ?? "";
                    var templateFileName = (string)container.Element(ManifestNamespace + "TemplateFileName") ?? "";
                    var folder = Path.Combine(Path.GetDirectoryName(templateManifest), relativePath);
                    var path = Path.Combine(folder, templateFileName);
                    var data = container.Element(ManifestNamespace + "VSTemplateHeader")?.Element(TemplateNamespace + "TemplateData");

                    if (data is null || !File.Exists(path))
                        continue;

                    var platforms = TagValues(data, "PlatformTag").ToList();

                    if (!platforms.Contains(TwinCatPlatformTag, StringComparer.OrdinalIgnoreCase))
                        continue;

                    var icon = (string)data.Element(TemplateNamespace + "Icon");
                    int.TryParse((string)data.Element(TemplateNamespace + "SortOrder"), out var sortOrder);

                    var category = ExtensionCategory((string)data.Element(TemplateNamespace + "ProjectType"), extensionFolder, extensionName);

                    var template = new TwinCatProjectTemplate(
                        path,
                        category,
                        ExtensionCategoryOrders.TryGetValue(category, out var order) ? order : OtherExtensionCategoryOrder,
                        platforms,
                        projectTypes: TagValues(data, "ProjectTypeTag"),
                        name: TemplateText(data.Element(TemplateNamespace + "Name")),
                        description: TemplateText(data.Element(TemplateNamespace + "Description")),
                        baseName: TemplateText(data.Element(TemplateNamespace + "DefaultName")),
                        iconPath: string.IsNullOrEmpty(icon) ? null : Path.Combine(folder, icon),
                        subCategory: ExtensionSubCategory(relativePath, templateFileName, category));

                    entries.Add((sortOrder, template));
                }
            }
            catch (Exception)
            {
                // an unreadable template manifest only hides its templates
            }

            return entries;
        }

        // 'TwinCAT Connectivity' names its ProjectType; Measurement says 'CSharp' and is grouped by its
        // product folder (...\TwinCAT Measurement\TE130x-Scope-View) like the shell's New Project dialog
        private static string ExtensionCategory(string projectType, string extensionFolder, string extensionName)
        {
            if (!string.IsNullOrWhiteSpace(projectType) && projectType.IndexOf(TwinCatPlatformTag, StringComparison.OrdinalIgnoreCase) >= 0)
                return projectType.Trim();

            var productFolder = Path.GetFileName(Path.GetDirectoryName(extensionFolder));

            if (!string.IsNullOrEmpty(productFolder) && productFolder.StartsWith(TwinCatPlatformTag, StringComparison.OrdinalIgnoreCase))
                return productFolder;

            return string.IsNullOrWhiteSpace(extensionName) ? TwinCatPlatformTag : extensionName;
        }

        // the template's folder path is its position in the tree - 'Scope\YT Scope Project.vstemplate' sits
        // under 'Scope'; a folder named after the template is its own folder, not a node
        // ('Framework\FrameworkPrj_PackagesConfig\FrameworkPrj.vstemplate' sits under 'Framework')
        private static string ExtensionSubCategory(string relativePath, string templateFileName, string category)
        {
            var segments = relativePath.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            var templateName = Path.GetFileNameWithoutExtension(templateFileName);

            if (segments.Count > 0 && segments[segments.Count - 1].IndexOf(templateName, StringComparison.OrdinalIgnoreCase) >= 0)
                segments.RemoveAt(segments.Count - 1);

            var subCategory = string.Join(@"\", segments);

            if (subCategory.Length == 0 || string.Equals(subCategory, category, StringComparison.OrdinalIgnoreCase))
                return null;

            return subCategory;
        }

        // skips folders that can't be read instead of failing the whole search
        private static IEnumerable<string> EnumerateFilesSafe(string root, string fileName)
        {
            var folders = new Stack<string>();
            folders.Push(root);

            while (folders.Count > 0)
            {
                var folder = folders.Pop();
                string[] files = Array.Empty<string>();
                string[] subfolders = Array.Empty<string>();

                try
                {
                    files = Directory.GetFiles(folder, fileName);
                    subfolders = Directory.GetDirectories(folder);
                }
                catch (Exception)
                {
                    // no access - skip this folder
                }

                foreach (var file in files)
                    yield return file;

                foreach (var subfolder in subfolders)
                    folders.Push(subfolder);
            }
        }

        private static IEnumerable<string> TagValues(XElement templateData, string tagName)
            => templateData.Elements(TemplateNamespace + tagName)
                .Select(e => e.Value.Trim())
                .Where(value => value.Length > 0)
                .ToList();

        // <Name>text</Name>, or <Description Package="{guid}" ID="150" /> for a package string resource
        private static string TemplateText(XElement element)
        {
            if (element is null)
                return null;

            if (Guid.TryParse((string)element.Attribute("Package"), out var package) && element.Attribute("ID") is XAttribute id)
                return LoadPackageString(package, "#" + id.Value);

            return element.Value;
        }

        // .vsdir line: RelPathName|{Package}|Name|SortPriority|Description|DllPath|IconId|Flags|SuggestedBaseName
        private static List<TwinCatProjectTemplate> ReadVsDirTemplates(string folder, string category, int categoryOrder, string[] projectTypes)
        {
            var entries = new List<(int Priority, TwinCatProjectTemplate Template)>();

            foreach (var vsdir in Directory.EnumerateFiles(folder, "*.vsdir"))
            {
                foreach (var line in File.ReadAllLines(vsdir))
                {
                    var fields = line.Split('|');
                    var path = Path.Combine(folder, Field(fields, 0));

                    if (fields.Length < 3 || !File.Exists(path))
                        continue;

                    Guid.TryParse(Field(fields, 1), out var package);
                    int.TryParse(Field(fields, 3), out var priority);

                    var template = new TwinCatProjectTemplate(
                        path,
                        category,
                        categoryOrder,
                        TwinCatPlatform,
                        projectTypes,
                        name: LoadPackageString(package, Field(fields, 2)),
                        description: LoadPackageString(package, Field(fields, 4)),
                        baseName: LoadPackageString(package, Field(fields, 8)));

                    entries.Add((priority, template));
                }
            }

            return entries.OrderBy(e => e.Priority).Select(e => e.Template).ToList();
        }

        private static string Field(string[] fields, int index)
            => (index < fields.Length) ? fields[index].Trim() : "";

        // '#127' is a string resource of the package; anything else is literal text
        private static string LoadPackageString(Guid package, string value)
        {
            if (!value.StartsWith("#"))
                return value;

            if (!ThreadHelper.CheckAccess() || !uint.TryParse(value.Substring(1), out var resourceId))
                return null; // the template falls back to its file name

            try
            {
#pragma warning disable VSTHRD010 // guarded by CheckAccess above, which the analyzer doesn't recognise
                if (Package.GetGlobalService(typeof(SVsShell)) is IVsShell shell
                    && ErrorHandler.Succeeded(shell.LoadPackageString(ref package, resourceId, out var text)))
                {
                    return text;
                }
#pragma warning restore VSTHRD010
            }
            catch (Exception)
            {
                // the package's resources aren't available
            }

            return null;
        }

        public static ImageSource LoadIcon(string templatePath, string iconPath = null)
        {
            try
            {
                if (iconPath is null || !File.Exists(iconPath))
                    iconPath = Path.ChangeExtension(templatePath, ".ico");

                // e.g. 'TwinCAT PLC Project.tspproj' comes with 'Standard PLC Template.ico'
                if (!File.Exists(iconPath))
                    iconPath = Directory.EnumerateFiles(Path.GetDirectoryName(templatePath), "*.ico").FirstOrDefault();

                if (iconPath is null)
                    return null;

                var decoder = new IconBitmapDecoder(new Uri(iconPath), BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                var frame = decoder.Frames.Where(f => f.PixelWidth >= 64).OrderBy(f => f.PixelWidth).FirstOrDefault()
                    ?? decoder.Frames.OrderByDescending(f => f.PixelWidth).First();

                frame.Freeze();

                return frame;
            }
            catch (Exception)
            {
                return null; // the item falls back to its KnownMonikers icon
            }
        }

        public static string NextFreeName(string location, string baseName)
        {
            for (var i = 1; i < 10000; i++)
            {
                var name = baseName + i;

                try
                {
                    if (!Directory.Exists(Path.Combine(location, name)))
                        return name;
                }
                catch (ArgumentException)
                {
                    return name; // invalid location - the dialog reports that separately
                }
            }

            return baseName;
        }

        // returns null when the input is valid, otherwise the message to show
        public static string Validate(string location, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Enter a project name.";

            if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 || name.EndsWith("."))
                return "The project name contains characters that are not allowed in a file name.";

            if (string.IsNullOrWhiteSpace(location))
                return "Enter a location.";

            if (location.IndexOfAny(Path.GetInvalidPathChars()) >= 0 || !Path.IsPathRooted(location))
                return "Enter the full path of a folder as the location.";

            var solutionFolder = Path.Combine(location, name);

            if (Directory.Exists(solutionFolder) && Directory.EnumerateFileSystemEntries(solutionFolder).Any())
                return $"The folder '{solutionFolder}' already exists and is not empty.";

            return null;
        }

        // category: the one the dialog opens in; null for the default template's ('TwinCAT Projects')
        public static bool CreateInteractively(DTE2 dte, string category = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var templates = FindTemplates();

            if (templates.Count == 0)
                throw new FileNotFoundException("No TwinCAT project templates were found. Is TwinCAT XAE installed?", string.Join("; ", TemplateFolders.Select(f => f.Folder)));

            var dialog = new CreateTwinCatProjectDialog(templates, DefaultLocation(dte), category);

            if (dialog.ShowModal() != true)
                return false;

            if (!CloseOpenSolution(dte))
                return false; // the user cancelled saving the open solution

            Create(dte, dialog.SelectedTemplate.Path, dialog.ProjectLocation, dialog.ProjectName);

            return true;
        }

        // same layout as the New Project dialog: <location>\<name>\<name>.sln + <name>\<name>.tsproj
        private static void Create(DTE2 dte, string template, string location, string name)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var solutionFolder = Path.Combine(location, name);

            Directory.CreateDirectory(solutionFolder);

            dte.Solution.Create(solutionFolder, name);
            dte.Solution.AddFromTemplate(template, Path.Combine(solutionFolder, name), name, false);
            dte.Solution.SaveAs(Path.Combine(solutionFolder, name + ".sln"));
        }

        private static bool CloseOpenSolution(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (!dte.Solution.IsOpen)
                return true;

            try
            {
                dte.ExecuteCommand(FILE_CLOSE_SOLUTION); // prompts to save pending changes
            }
            catch (COMException)
            {
                // thrown when the save prompt is cancelled
            }

            return !dte.Solution.IsOpen;
        }

        private static string DefaultLocation(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var configured = Environment.ExpandEnvironmentVariables(StartItemsOptions.Instance.TwinCatProjectLocation ?? "").Trim();

            // a configured folder that doesn't exist yet is fine - creating the project creates it
            if (configured.Length > 0 && configured.IndexOfAny(Path.GetInvalidPathChars()) < 0 && Path.IsPathRooted(configured))
                return configured;

            return ShellProjectsLocation(dte);
        }

        // Tools > Options > Projects and Solutions > Locations > Projects location
        public static string ShellProjectsLocation(DTE2 dte)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                var location = dte?.Properties["Environment", "ProjectsAndSolution"].Item("ProjectsLocation").Value as string;

                if (!string.IsNullOrEmpty(location) && Directory.Exists(location))
                    return location;
            }
            catch (Exception)
            {
                // the setting isn't available - fall back to the documents folder
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        // the machine-wide install folder; the per-user InstallDir in HKCU can be stale
        private static string ReadInstallDir()
        {
            try
            {
                using (var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
                using (var key = baseKey.OpenSubKey(@"SOFTWARE\Beckhoff\TwinCAT3\3.1"))
                {
                    return key?.GetValue("InstallDir") as string;
                }
            }
            catch (Exception)
            {
                return null; // no access to the registry
            }
        }
    }
}
