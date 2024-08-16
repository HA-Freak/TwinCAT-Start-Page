namespace StartPageTcXaeShell64.Core.Interfaces
{
    using StartPageTcXaeShell64.UI.ViewModels.NewsItems;

    public interface INewsItemActionService
    {
        void DoAction(NewsItemViewModel currentViewModel, bool internalBrowser = true);
    }
}