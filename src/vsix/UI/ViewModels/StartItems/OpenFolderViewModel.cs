using Microsoft.VisualStudio.Imaging;

namespace TwinCAT.XAE.StartPage.UI.ViewModels.StartItems
{
    using Interfaces;

    using ViewModels.RecentItems;

    public class OpenFolderViewModel : StartItemViewModel
    {
        public OpenFolderViewModel(IStartItemActionService actionService) : base(actionService)
        {
            Moniker = KnownMonikers.OpenFolder;
            Name = "Open a local folder";
            Description = "Navigate and edit code within any folder on your machine or network";
        }

        // the start page is hidden by EventManager when a folder actually opens,
        // so cancelling the dialog leaves the start page untouched
        protected override void OnClick()
            => ActionService.OpenFolder();
    }
}