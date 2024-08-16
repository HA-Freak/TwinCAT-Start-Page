using Microsoft.VisualStudio.Imaging;

namespace StartPageTcXaeShell64.UI.ViewModels.StartItems
{
    using Interfaces;

    using ViewModels.RecentItems;

    using EnvDTE;
    using EnvDTE80;
    using Community.VisualStudio.Toolkit;

    public class CreateProjectViewModel : StartItemViewModel
    {
        public CreateProjectViewModel(IStartItemActionService actionService) : base(actionService)
        {
            Moniker = KnownMonikers.AddDocument;  // AddDocumentGroup missing from KnownMonikers in SDK v15.9.3
            Name = "Create a new project";
            Description = "Choose a project template with code scaffolding to get started";
        }

        private static DTE2 Dte
            => VS.GetRequiredService<_DTE, DTE2>(); //YD: investigate toolkit for DTE2 alternative, also a way to not call every service (some may never be used)

        protected override void OnClick()
        {
            ActionService.CreateNewProject();

            var StartPageId = Dte.ActiveWindow.ObjectKind;
            Dte.Windows.Item(StartPageId).Close();//SRS
        }
           
    }
}