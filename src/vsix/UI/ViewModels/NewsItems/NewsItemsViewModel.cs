using System;
using System.Collections.Generic;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace TwinCAT.XAE.StartPage.UI.ViewModels.NewsItems
{
    using Core.Interfaces;

    using Interfaces;
    using Interfaces.NewsItems;

    using Messages;

    using Options.Pages;

    //YD: for future development
    //#region Assembly System.ServiceModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
    //namespace System.ServiceModel.Syndication
    //public class SyndicationFeed

    public partial class NewsItemsViewModel : ColumnViewModel
    {
        private const string HEADING = "News";

        private List<NewsItemViewModel> _items = new();
        private NewsItemViewModel _selectedItem;
        private bool _refreshed;
        private bool _minimizedByUser;
        private bool _tooNarrow;
        private bool _expandedAnyway;
        private readonly IDialogService _dialogService;
        private readonly IVisualStudioService _visualStudioService;

        //---

        public NewsItemsViewModel(
            INewsItemDataService dataService,
            INewsItemCommandService commandService,
            INewsItemActionService actionService,
            IDialogService dialogService,
            IVisualStudioService visualStudioService
            )
        {
            DataService = dataService;
            CommandService = commandService;
            ActionService = actionService;
            _dialogService = dialogService;
            _visualStudioService = visualStudioService;

            Heading = HEADING;
            IsVisible = true;
            _minimizedByUser = NewsItemsOptions.Instance.NewsItemsMinimized;

            GetCommands();
            Refresh();

            ListenFor<RefreshNewsItems>(this, RefreshItems);
            ListenFor<NewsItemSelected>(this, OnItemSelected);
        }

        //---

        public INewsItemDataService DataService { get; }

        public INewsItemCommandService CommandService { get; }

        public INewsItemActionService ActionService { get; }

        public List<NewsItemViewModel> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }

        public NewsItemViewModel SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public bool Refreshed
        {
            get => _refreshed;
            set
            {
                if (SetProperty(ref _refreshed, value) && (value == true))
                {
                    SelectedItem = null;

                    Messenger.Send(new NewsItemsRefreshed());
                }
            }
        }

        // only the vertical 'News' strip at the right edge is shown:
        // minimized by the user (remembered), or automatically while the start page is too narrow
        public bool IsMinimized
            => _minimizedByUser || (_tooNarrow && !_expandedAnyway);

        // set by the start view when its width changes
        public bool IsTooNarrow
        {
            get => _tooNarrow;
            set
            {
                if (_tooNarrow == value)
                    return;

                _tooNarrow = value;
                _expandedAnyway = false;
                OnPropertyChanged(nameof(IsMinimized));
            }
        }

        //---

        // only the user's own choice is remembered, never the automatic minimizing
        [RelayCommand]
        private void ToggleMinimized()
        {
            if (IsMinimized)
            {
                if (_minimizedByUser)
                    SetMinimizedByUser(false);

                // opened although there isn't enough space: stays open until the width changes
                if (_tooNarrow)
                    _expandedAnyway = true;
            }
            else if (_tooNarrow)
            {
                _expandedAnyway = false;
            }
            else
            {
                SetMinimizedByUser(true);
            }

            OnPropertyChanged(nameof(IsMinimized));
        }

        private void SetMinimizedByUser(bool minimized)
        {
            _minimizedByUser = minimized;
            NewsItemsOptions.Instance.SaveMinimized(minimized);
        }

        public void Refresh()
        {
            Refreshed = false;

            try
            {
                var _options = NewsItemsOptions.Instance;

                if (_options.ClearListBeforeRefresh)
                    Items.Clear();

                var itemsToDisplay = _options.NewsItemsToDisplay;
                var url = _options.NewsItemsFeedUrl;

                Run(
                    async () => Items = await DataService.GetItemsAsync(url, itemsToDisplay)
                );

                Refreshed = true;
            }
            catch (Exception ex)
            {
                _dialogService.ShowException(ex);
            }
            finally
            {
                SelectedItem = null;
            }
        }

        //---

        private void GetCommands()
            => Commands = CommandService.GetCommands(Refresh, OpenSettings);

        private void RefreshItems(object Recipient, RefreshNewsItems message)
            => Refresh();

        private void OpenSettings()
            => _visualStudioService.ShowOptions<OptionsProvider.NewsItems>();

        private void OnItemSelected(object recipient, NewsItemSelected message)
        {
            var openInVS = NewsItemsOptions.Instance.OpenLinksInVS;

            ActionService.DoAction(message.Value, openInVS);
        }
    }
}