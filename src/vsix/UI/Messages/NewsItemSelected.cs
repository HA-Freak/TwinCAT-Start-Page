using CommunityToolkit.Mvvm.Messaging.Messages;

namespace StartPageTcXaeShell64.UI.Messages
{
    using ViewModels.NewsItems;

    public sealed class NewsItemSelected : ValueChangedMessage<NewsItemViewModel>
    {
        public NewsItemSelected(NewsItemViewModel viewModel) : base(viewModel)
        { }
    }
}