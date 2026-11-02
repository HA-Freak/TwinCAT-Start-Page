namespace TwinCAT.XAE.StartPage.UI.Interfaces
{
    using ViewModels.RecentItems;

    public interface IRecentItemActionService
    {
        void ExecuteAction(RecentItemViewModel viewModel);
    }
}