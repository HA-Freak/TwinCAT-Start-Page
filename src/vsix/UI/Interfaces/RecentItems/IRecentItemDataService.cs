namespace StartPageTcXaeShell64.UI.Interfaces.RecentItems
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;

    using StartPageTcXaeShell64.UI.ViewModels.RecentItems;

    public interface IRecentItemDataService
    {
        Task<ObservableCollection<RecentItemViewModel>> GetItemsAsync(int itemsToDisplay, bool showExtensions, bool showPaths, bool showCsProjFiles);

        Task<bool> RemoveItemAsync(RecentItemViewModel viewModel);

        Task<bool> PinItemAsync(RecentItemViewModel viewModel);

        Task<bool> UnpinItemAsync(RecentItemViewModel viewModel);

        Task<bool> UpdateItemPathAsync(RecentItemViewModel viewModel, string value);

        Task<bool> CopyItemPathAsync(RecentItemViewModel viewModel);

        Task<bool?> EditItemPathAsync(RecentItemViewModel viewModel);

        Task<bool> OpenInVsAsync(RecentItemViewModel viewModel);

        Task<bool> UpdateLastAccessedAsync(string path);
    }
}