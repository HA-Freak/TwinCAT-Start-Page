using Microsoft.VisualStudio.Imaging;

namespace TwinCAT.XAE.StartPage.UI.ViewModels.StartItems
{
    using Interfaces;

    using ViewModels.RecentItems;

    public class CreateProjectViewModel : StartItemViewModel
    {
        public CreateProjectViewModel(IStartItemActionService actionService) : base(actionService)
        {
            Moniker = KnownMonikers.AddDocument;  // AddDocumentGroup missing from KnownMonikers in SDK v15.9.3
            Name = "Create a new project";
            Description = "Choose a project template with code scaffolding to get started";
        }

        // the start page is hidden by EventManager when a solution actually opens,
        // so cancelling the dialog leaves the start page untouched
        protected override void OnClick()
            => ActionService.CreateNewProject();
    }
}