namespace TwinCAT.XAE.StartPage.UI.ViewModels
{
    using Options.Pages;

    using TwinCAT.XAE.StartPage.UI.ViewModels.NewsItems;
    using TwinCAT.XAE.StartPage.UI.ViewModels.RecentItems;
    using TwinCAT.XAE.StartPage.UI.ViewModels.StartItems;

    public class StartViewModel : TabViewModel
    {
        public StartViewModel(RecentItemsViewModel recentItemsViewModel, StartItemsViewModel startItemsViewModel, NewsItemsViewModel newsItemsViewModel) : base()
        {
            var options = GeneralOptions.Instance;

            Name = "Start";
            Title = options.StartTabTitleText;

            RecentItems = recentItemsViewModel;
            StartItems = startItemsViewModel;

            if (options.DisplayNewsItems)
            {
                NewsItems = newsItemsViewModel;
            }
        }

        //---

        public RecentItemsViewModel RecentItems { get; }

        public StartItemsViewModel StartItems { get; }

        // null when the news column is turned off in the options
        public NewsItemsViewModel NewsItems { get; }
    }
}
