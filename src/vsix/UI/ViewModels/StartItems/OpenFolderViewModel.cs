using Microsoft.VisualStudio.Imaging;

namespace StartPageTcXaeShell64.UI.ViewModels.StartItems
{
    using Interfaces;

    using ViewModels.RecentItems;

    using EnvDTE;
    using EnvDTE80;
    using Community.VisualStudio.Toolkit;

    public class OpenFolderViewModel : StartItemViewModel
    {
        public OpenFolderViewModel(IStartItemActionService actionService) : base(actionService)
        {
            Moniker = KnownMonikers.OpenFolder;
            Name = "Open a local folder";
            Description = "Navigate and edit code within any folder on your machine or network";
        }

        private static DTE2 Dte
            => VS.GetRequiredService<_DTE, DTE2>(); //YD: investigate toolkit for DTE2 alternative, also a way to not call every service (some may never be used)

        protected override void OnClick()
        {
            ActionService.OpenFolder();

            var StartPageId = Dte.ActiveWindow.ObjectKind;
            Dte.Windows.Item(StartPageId).Close();//SRS
        }
    }
}