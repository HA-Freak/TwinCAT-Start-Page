namespace TwinCAT.XAE.StartPage.UI.Views
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Microsoft.VisualStudio.PlatformUI;

    using Services.Other;

    public partial class CreateTwinCatProjectDialog : DialogWindow
    {
        private const string AllProjectTypes = "All project types";

        private readonly IReadOnlyList<TwinCatProjectTemplate> _templates;
        private TemplateCategoryNode _selectedNode;
        private (string Category, string SubCategory)? _categoryBeforeSearch;
        private string _suggestedName = "";
        private bool _filtering;

        public CreateTwinCatProjectDialog(IReadOnlyList<TwinCatProjectTemplate> templates, string location, string category = null)
        {
            InitializeComponent();

            _templates = templates;

            LocationBox.Text = location;

            _filtering = true;
            TypeFilter.ItemsSource = new[] { AllProjectTypes }
                .Concat(templates.SelectMany(t => t.ProjectTypes).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(t => t, StringComparer.CurrentCultureIgnoreCase))
                .ToList();
            TypeFilter.SelectedIndex = 0;
            _filtering = false;

            // without a category, open where the default template is ('TwinCAT XAE Project', the first one)
            BuildTree(category ?? templates.FirstOrDefault()?.Category, preferredSubCategory: null, selectSearchResults: false);

            NameBox.SelectAll();

            Validate();
        }

        public TwinCatProjectTemplate SelectedTemplate
            => TemplateList.SelectedItem as TwinCatProjectTemplate;

        public string ProjectName
            => NameBox.Text.Trim();

        public string ProjectLocation
            => LocationBox.Text.Trim();

        private void OnInputChanged(object sender, TextChangedEventArgs e)
            => Validate();

        private void OnLocationChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSuggestedName();
            Validate();
        }

        private bool IsSearching
            => !string.IsNullOrWhiteSpace(SearchBox?.Text);

        // like the shell's New Project dialog: typing selects a 'Search Results' node that lists the matches
        // of all categories; clearing the search goes back to the category that was selected before
        private void OnSearchChanged(object sender, TextChangedEventArgs e)
        {
            var empty = string.IsNullOrEmpty(SearchBox.Text);

            SearchPlaceholder.Visibility = empty ? Visibility.Visible : Visibility.Collapsed;
            ClearSearchButton.Visibility = empty ? Visibility.Collapsed : Visibility.Visible;

            if (_filtering || _templates is null)
                return;

            if (IsSearching)
            {
                if (_selectedNode != null && !_selectedNode.IsSearchResults)
                    _categoryBeforeSearch = (_selectedNode.Category, _selectedNode.SubCategory);

                BuildTree(_selectedNode?.Category, _selectedNode?.SubCategory, selectSearchResults: true);
            }
            else
            {
                var previous = _categoryBeforeSearch;
                _categoryBeforeSearch = null;

                BuildTree(previous?.Category ?? _templates.FirstOrDefault()?.Category, previous?.SubCategory, selectSearchResults: false);
            }
        }

        private void OnSearchKeyDown(object sender, KeyEventArgs e)
        {
            // Esc clears the search first; only an empty search lets Esc close the dialog
            if (e.Key == Key.Escape && !string.IsNullOrEmpty(SearchBox.Text))
            {
                SearchBox.Clear();
                e.Handled = true;
            }
        }

        private void OnClearSearch(object sender, RoutedEventArgs e)
        {
            SearchBox.Clear();
            SearchBox.Focus();
        }

        private void OnTypeChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_filtering && _templates != null)
                BuildTree(_selectedNode?.Category, _selectedNode?.SubCategory, selectSearchResults: _selectedNode?.IsSearchResults == true);
        }

        // the tree lists the categories that have templates of the selected project type, plus
        // 'Search Results' while searching; the selection stays where it was if it still exists
        private void BuildTree(string preferredCategory, string preferredSubCategory, bool selectSearchResults)
        {
            var comparer = StringComparer.CurrentCultureIgnoreCase;

            var nodes = _templates
                .Where(MatchesType)
                .GroupBy(t => t.Category)
                .OrderBy(g => g.Min(t => t.CategoryOrder))
                .ThenBy(g => g.Key, comparer)
                .Select(g => new TemplateCategoryNode(
                    g.Key,
                    subCategory: null,
                    g.Select(t => t.SubCategory)
                        .Where(s => s != null)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(s => s, comparer)
                        .Select(s => new TemplateCategoryNode(g.Key, s, Array.Empty<TemplateCategoryNode>()))
                        .ToList()))
                .ToList();

            var searchResults = IsSearching ? TemplateCategoryNode.CreateSearchResults() : null;

            if (searchResults != null)
                nodes.Add(searchResults);

            var target = (selectSearchResults ? searchResults : null)
                ?? nodes
                    .SelectMany(n => new[] { n }.Concat(n.Children))
                    .FirstOrDefault(n => !n.IsSearchResults && n.Category == preferredCategory && n.SubCategory == preferredSubCategory)
                ?? nodes.FirstOrDefault();

            _filtering = true;
            CategoryTree.ItemsSource = nodes;
            _selectedNode = target;

            if (target != null)
                target.IsSelected = true; // the tree applies it once the node's container exists

            _filtering = false;

            ShowTemplates();
        }

        private void OnCategoryChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            // rebuilding the tree briefly reports 'nothing selected' - keep the node ApplyFilter picked
            if (e.NewValue is TemplateCategoryNode node)
            {
                _selectedNode = node;

                if (!_filtering)
                    ShowTemplates();
            }
        }

        private void ShowTemplates()
        {
            var node = _selectedNode;
            var selected = SelectedTemplate;

            var templates = (node is null)
                ? new List<TwinCatProjectTemplate>()
                : node.IsSearchResults
                    ? _templates.Where(t => MatchesType(t) && MatchesSearch(t)).OrderBy(t => t.CategoryOrder).ToList()
                    : _templates.Where(t => node.Contains(t) && MatchesType(t)).ToList();

            TemplateList.ItemsSource = templates;
            TemplateList.SelectedItem = templates.Contains(selected) ? selected : templates.FirstOrDefault();
            NoMatchText.Visibility = (templates.Count == 0) ? Visibility.Visible : Visibility.Collapsed;

            ShowSelectedTemplate();
        }

        private bool MatchesType(TwinCatProjectTemplate template)
            => !(TypeFilter.SelectedItem is string type)
                || type == AllProjectTypes
                || template.ProjectTypes.Contains(type, StringComparer.OrdinalIgnoreCase);

        // every word of the search has to appear in the name, description, category or a tag
        private bool MatchesSearch(TwinCatProjectTemplate template)
        {
            var words = (SearchBox.Text ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var text = string.Join(" ", new[] { template.Name, template.Description, template.Category }.Concat(template.Tags));

            return words.All(word => text.IndexOf(word, StringComparison.CurrentCultureIgnoreCase) >= 0);
        }

        private void OnTemplateChanged(object sender, SelectionChangedEventArgs e)
            => ShowSelectedTemplate();

        private void ShowSelectedTemplate()
        {
            if (DescriptionText is null)
                return;

            DescriptionText.Text = SelectedTemplate?.Description ?? "";

            UpdateSuggestedName();
            Validate();
        }

        // keeps the name in step with the template and location until the user types their own
        private void UpdateSuggestedName()
        {
            if (SelectedTemplate is null || NameBox is null || LocationBox is null)
                return;

            if (ProjectName.Length > 0 && ProjectName != _suggestedName)
                return;

            _suggestedName = TwinCatProject.NextFreeName(ProjectLocation, SelectedTemplate.BaseName);
            NameBox.Text = _suggestedName;
        }

        private void Validate()
        {
            if (CreateButton is null || NameBox is null || LocationBox is null)
                return; // change events can fire while InitializeComponent is still running

            var error = (SelectedTemplate is null)
                ? "Select a template."
                : TwinCatProject.Validate(ProjectLocation, ProjectName);

            CreateButton.IsEnabled = error is null;
            MessageText.Text = error ?? $"The solution will be created in '{Path.Combine(ProjectLocation, ProjectName)}'.";
        }

        private void OnBrowse(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = "Select the folder the project is created in";
                dialog.ShowNewFolderButton = true;

                if (Directory.Exists(ProjectLocation))
                {
                    dialog.SelectedPath = ProjectLocation;
                }

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    LocationBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void OnCreate(object sender, RoutedEventArgs e)
            => DialogResult = true;
    }

    // a node of the category tree: a category ('TwinCAT Measurement'), one of its sub-categories ('Scope'),
    // or 'Search Results' while searching
    public sealed class TemplateCategoryNode : INotifyPropertyChanged
    {
        private const string SearchResultsName = "Search Results";

        private bool _isSelected;

        public TemplateCategoryNode(string category, string subCategory, IReadOnlyList<TemplateCategoryNode> children)
        {
            Category = category;
            SubCategory = subCategory;
            Children = children;
        }

        public static TemplateCategoryNode CreateSearchResults()
            => new TemplateCategoryNode(null, null, Array.Empty<TemplateCategoryNode>()) { IsSearchResults = true };

        public event PropertyChangedEventHandler PropertyChanged;

        public string Category { get; }

        public string SubCategory { get; }

        public bool IsSearchResults { get; private set; }

        public string Name
            => IsSearchResults ? SearchResultsName : SubCategory ?? Category;

        public IReadOnlyList<TemplateCategoryNode> Children { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value)
                    return;

                _isSelected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
            }
        }

        // a category shows its own templates and those of its sub-categories
        public bool Contains(TwinCatProjectTemplate template)
            => template.Category == Category && (SubCategory is null || template.SubCategory == SubCategory);
    }
}
