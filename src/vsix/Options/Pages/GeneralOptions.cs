using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Community.VisualStudio.Toolkit;

namespace TwinCAT.XAE.StartPage.Options.Pages
{
    using Core.Interfaces;

    using TwinCAT.XAE.StartPage.UI.Services;

    using UI.ToolWindows;

    using static TwinCAT.XAE.StartPage.Options.Pages.PageConstants;

    internal partial class OptionsProvider
    {
        [ComVisible(true)]
        public class General : BaseOptionPage<GeneralOptions> { }
    }

    public class GeneralOptions : BaseOptionModel<GeneralOptions>
    {
        public const string Category = "General";
        private readonly IDialogService _dialogService;

        //---

        public GeneralOptions() : base()
        {
            _dialogService = ServiceManager.DialogService;

            Saved += OnSaved;
        }

        //---

        [Category(H1 + PackageName)]
        [DisplayName(EnableStartPageDispayName)]
        [Description(EnableStartPageDescription)]
        public bool EnableStartPage { get; set; } = EnableStartPageDefault;

        [Category(H1 + PackageName)]
        [DisplayName(EnableOptionsMenuItemDispayName)]
        [Description(EnableOptionsMenuItemDescription)]
        public bool EnableStartPageOptions { get; set; } = EnableStartPageOptionsDefault;

        [Category(H1 + PackageName)]
        [DisplayName(PageConstants.PackageVersion)]
        [Description("Installed '" + PackageName + "' version")]
        public string PackageVersion { get; } = Vsix.Version;

        //--- appearance

        [Category(H2 + PageConstants.Appearance)]
        [DisplayName(MaxWidthDisplayName)]
        [Description(MaxWidthDescription)]
        public int MaxWidth { get; set; } = MaxWidthDefault;

        [Category(H2 + PageConstants.Appearance)]
        [DisplayName(ShowStartTabTitleDisplayName)]
        [Description(ShowStartTabTitleDescription)]
        public bool ShowStartTabTitle { get; set; } = ShowStartTabTitleDefault;

        [Category(H2 + PageConstants.Appearance)]
        [DisplayName(StartTabTitleTextDisplayName)]
        [Description(StartTabTitleTextDescription)]
        public string StartTabTitleText { get; set; } = StartTabTitleDefault;

        [Category(H2 + PageConstants.Appearance)]
        [DisplayName(DisplayNewsItemsName)]
        [Description(DisplayNewsItemsDescription)]
        public bool DisplayNewsItems { get; set; } = DisplayNewsItemsDefault;

        //--- behavior

        [Category(H3 + PageConstants.Behavior)]
        [DisplayName(ShowOnStartupDisplayName)]
        [Description(ShowOnStartupDescription)]
        public bool ShowOnStartup { get; set; } = ShowOnStartupDefault;

        [Category(H3 + PageConstants.Behavior)]
        [DisplayName(HideOnSolutionOpenDisplayName)]
        [Description(HideOnSolutionOpenDescription)]
        public bool HideOnSolutionOpen { get; set; } = HideOnSolutionOpenDefault;

        [Category(H3 + PageConstants.Behavior)]
        [DisplayName(RestoreOnSolutionCloseDisplayName)]
        [Description(RestoreOnSolutionCloseDescription)]
        public bool RestoreOnSolutionClose { get; set; } = RestoreOnSolutionCloseDefault;

        //---

        private void OnSaved(GeneralOptions options)
        {
            try
            {
                // View is only set once the tool window has been created
                MainWindow.View?.InvalidateVisual();
            }
            catch (Exception ex)
            {
                _dialogService?.ShowException(ex);
            }
        }
    }
}