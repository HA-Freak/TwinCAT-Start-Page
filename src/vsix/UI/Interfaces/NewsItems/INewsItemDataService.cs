namespace TwinCAT.XAE.StartPage.UI.Interfaces.NewsItems
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using TwinCAT.XAE.StartPage.UI.ViewModels.NewsItems;

    public interface INewsItemDataService
    {
        Task<List<NewsItemViewModel>> GetItemsAsync(string url, int itemsToDisplay);
    }
}