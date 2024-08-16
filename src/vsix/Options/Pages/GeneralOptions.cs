using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

using Community.VisualStudio.Toolkit;

namespace StartPageTcXaeShell64.Options.Pages
{
    using Core.Interfaces;

    using StartPageTcXaeShell64.UI.Services;

    using UI.ToolWindows;

    using static StartPageTcXaeShell64.Options.Pages.PageConstants;

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
        [DisplayName(EnableStartPageTcXaeShell64DispayName)]
        [Description(EnableStartPageTcXaeShell64Description)]
        public bool EnableStartPageTcXaeShell64 { get; set; } = EnableStartPageTcXaeShell64Default;

        [Category(H1 + PackageName)]
        [DisplayName(EnableOptionsMenuItemDispayName)]
        [Description(EnableOptionsMenuItemDescription)]
        public bool EnableStartPageTcXaeShell64Options { get; set; } = EnableStartPageTcXaeShell64OptionsDefault;

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
                MainWindow.View.InvalidateVisual();

            }
            catch (Exception ex)
            {
                _dialogService.ShowException(ex);
            }
        }
    }
}