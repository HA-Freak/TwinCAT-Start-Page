using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;

using Community.VisualStudio.Toolkit;

using CommunityToolkit.Mvvm.Messaging;

using EnvDTE;

using EnvDTE80;

using Microsoft.VisualStudio.Shell;

namespace TwinCAT.XAE.StartPage.Options.Pages
{
    using DI;

    using UI.Messages;
    using UI.Services.Other;

    using static Pages.PageConstants;

    //---

    internal partial class OptionsProvider
    {
        [ComVisible(true)]
        public class StartItems : BaseOptionPage<StartItemsOptions>
        {
            // an empty location means "the shell's projects location" - show that path instead of a blank field
            public override void LoadSettingsFromStorage()
            {
                base.LoadSettingsFromStorage();

                if (!ThreadHelper.CheckAccess())
                    return; // reading the shell's location needs the UI thread

                if (AutomationObject is StartItemsOptions options && string.IsNullOrWhiteSpace(options.TwinCatProjectLocation))
                {
#pragma warning disable VSTHRD010 // guarded by CheckAccess above, which the analyzer doesn't recognise
                    options.TwinCatProjectLocation = TwinCatProject.ShellProjectsLocation(VS.GetRequiredService<_DTE, DTE2>());
#pragma warning restore VSTHRD010
                }
            }
        }
    }

    //---

    public class StartItemsOptions : BaseOptionModel<StartItemsOptions>
    {
        public const string Category = "Start Items";

        //---

        public StartItemsOptions() : base()
        {
            Saved += OnSaved;
        }

        //---

        [Category(H1 + PageConstants.Behavior)]
        [DisplayName(CloneRepositoryDelayDisplayName)]
        [Description(CloneRepositoryDelayDescription)]
        public int CloneRepositoryDelay { get; set; } = CloneRepositoryDelayDefault;

        [Category(H1 + PageConstants.Behavior)]
        [DisplayName(OpenFolderDelayDisplayName)]
        [Description(OpenFolderDelayDescription)]
        public int OpenFolderDelay { get; set; } = OpenFolderDelayDefault;

        [Category(H1 + PageConstants.Behavior)]
        [DisplayName(OpenProjectDelayDisplayName)]
        [Description(OpenProjectDelayDescription)]
        public int OpenProjectDelay { get; set; } = OpenProjectDelayDefault;

        [Category(H1 + PageConstants.Behavior)]
        [DisplayName(CreateProjectDelayDisplayName)]
        [Description(CreateProjectDelayDescription)]
        public int CreateProjectDelay { get; set; } = CreateProjectDelayDefault;

        [Category(H2 + TwinCatProjects)]
        [DisplayName(TwinCatProjectLocationDisplayName)]
        [Description(TwinCatProjectLocationDescription)]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public string TwinCatProjectLocation { get; set; } = TwinCatProjectLocationDefault;

        //---

        private void OnSaved(StartItemsOptions options)
        {
            var container = StartPageContainer.Instance;
            var messenger = container.GetInstance<IMessenger>();

            messenger.Send<RefreshStartItems>();
        }
    }
}