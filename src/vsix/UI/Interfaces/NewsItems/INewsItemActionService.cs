namespace TwinCAT.XAE.StartPage.Core.Interfaces
{
    using TwinCAT.XAE.StartPage.UI.ViewModels.NewsItems;

    public interface INewsItemActionService
    {
        void DoAction(NewsItemViewModel currentViewModel, bool internalBrowser = true);
    }
}