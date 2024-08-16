using Microsoft.VisualStudio.Imaging;

namespace StartPageTcXaeShell64.UI.ViewModels.StartItems
{
    using Interfaces;

    using ViewModels.RecentItems;

    using EnvDTE;
    using EnvDTE80;
    using Community.VisualStudio.Toolkit;

    public class OpenProjectViewModel : StartItemViewModel
    {
        public OpenProjectViewModel(IStartItemActionService actionService) : base(actionService)
        {
            Moniker = KnownMonikers.OpenTopic;  // OpenDocumentGroup missing from KnownMonikers in SDK v15.9.3
            Name = "Open a project or solution";
            Description = "Open a local Visual Studio project or a '.sln' file";
        }

        private static DTE2 Dte
            => VS.GetRequiredService<_DTE, DTE2>(); //YD: investigate toolkit for DTE2 alternative, also a way to not call every service (some may never be used)

        protected override void OnClick()
        {
            ActionService.OpenProject();

            var StartPageId = Dte.ActiveWindow.ObjectKind;
            Dte.Windows.Item(StartPageId).Close();//SRS
        }
    }
}