using Microsoft.VisualStudio.Imaging;

namespace TwinCAT.XAE.StartPage.UI.ViewModels.StartItems
{
    using Interfaces;

    using ViewModels.RecentItems;

    public class OpenProjectViewModel : StartItemViewModel
    {
        public OpenProjectViewModel(IStartItemActionService actionService) : base(actionService)
        {
            Moniker = KnownMonikers.OpenTopic;  // OpenDocumentGroup missing from KnownMonikers in SDK v15.9.3
            Name = "Open a project or solution";
            Description = "Open a local Visual Studio project or a '.sln' file";
        }

        // the start page is hidden by EventManager when a solution actually opens,
        // so cancelling the dialog leaves the start page untouched
        protected override void OnClick()
            => ActionService.OpenProject();
    }
}