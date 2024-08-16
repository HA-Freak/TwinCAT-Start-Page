namespace StartPageTcXaeShell64.UI.Interfaces.NewsItems
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using StartPageTcXaeShell64.UI.ViewModels.NewsItems;

    public interface INewsItemDataService
    {
        Task<List<NewsItemViewModel>> GetItemsAsync(string url, int itemsToDisplay);
    }
}