using System.ComponentModel;
using System.Runtime.InteropServices;

using Community.VisualStudio.Toolkit;

using CommunityToolkit.Mvvm.Messaging;

namespace TwinCAT.XAE.StartPage.Options.Pages
{
    using DI;

    using UI.Messages;

    using static Options.Pages.PageConstants;

    //---

    internal partial class OptionsProvider
    {
        [ComVisible(true)]
        public class NewsItems : BaseOptionPage<NewsItemsOptions> { }
    }

    //---

    public class NewsItemsOptions : BaseOptionModel<NewsItemsOptions>
    {
        public const string Category = @"News Items";

        //---

        public NewsItemsOptions() : base()
        {
            Saved += OnSaved;
        }

        //--- appearance

        [Category(H1 + PageConstants.Appearance)]
        [DisplayName(NewsItemsFeedUrlDisplayName)]
        [Description(NewsItemsFeedUrlDescription)]
        public string NewsItemsFeedUrl { get; set; } = NewsItemsFeedUrlDefault;

        [Category(H1 + PageConstants.Appearance)]
        [DisplayName(NewsItemsToDisplayDisplayName)]
        [Description(NewsItemsToDisplayDescription)]
        public int NewsItemsToDisplay { get; set; } = NewsItemsToDisplayDefault;

        //--- behavior

        [Category(H2 + PageConstants.Behavior)]
        [DisplayName(ClearListBeforeRefreshDisplayName)]
        [Description(ClearListBeforeRefreshDescription)]
        public bool ClearListBeforeRefresh { get; set; } = ClearListBeforeRefreshDefault;

        [Category(H2 + PageConstants.Behavior)]
        [DisplayName(OpenLinksInVsDisplayName)]
        [Description(OpenLinksInVsDescription)]
        public bool OpenLinksInVS { get; set; } = OpenLinksInVsDefault;

        // toggled on the start page, so it isn't shown in the options dialog
        [Browsable(false)]
        public bool NewsItemsMinimized { get; set; } = false;

        //---

        private bool _savingMinimized;

        // remembers the state without reloading the news feed
        public void SaveMinimized(bool minimized)
        {
            NewsItemsMinimized = minimized;
            _savingMinimized = true;

            try
            {
                Save();
            }
            finally
            {
                _savingMinimized = false;
            }
        }

        private void OnSaved(NewsItemsOptions options)
        {
            if (_savingMinimized)
                return;

            var container = StartPageContainer.Instance;
            var messenger = container.GetInstance<IMessenger>();

            messenger.Send<RefreshNewsItems>();
        }
    }
}