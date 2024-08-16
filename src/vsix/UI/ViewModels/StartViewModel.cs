using System.Collections.ObjectModel;

namespace StartPageTcXaeShell64.UI.ViewModels
{
    using Options.Pages;

    using StartPageTcXaeShell64.UI.ViewModels.NewsItems;
    using StartPageTcXaeShell64.UI.ViewModels.RecentItems;
    using StartPageTcXaeShell64.UI.ViewModels.StartItems;

    public class StartViewModel : TabViewModel
    {
        public StartViewModel(RecentItemsViewModel recentItemsViewModel, StartItemsViewModel startItemsViewModel, NewsItemsViewModel newsItemsViewModel) : base()
        {
            var options = GeneralOptions.Instance;

            Name = "Start";
            Title = options.StartTabTitleText;

            if (options.DisplayNewsItems)
            {
                Columns = new ObservableCollection<ColumnViewModel>
                {
                    recentItemsViewModel,
                    startItemsViewModel,
                    newsItemsViewModel
                };
            }
            else
            {
                Columns = new ObservableCollection<ColumnViewModel>
                {
                    recentItemsViewModel,
                    startItemsViewModel
                };
            }
        }

        //---

        public ObservableCollection<ColumnViewModel> Columns { get; set; }
    }
}