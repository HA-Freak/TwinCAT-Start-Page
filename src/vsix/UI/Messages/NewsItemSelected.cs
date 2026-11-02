using CommunityToolkit.Mvvm.Messaging.Messages;

namespace TwinCAT.XAE.StartPage.UI.Messages
{
    using ViewModels.NewsItems;

    public sealed class NewsItemSelected : ValueChangedMessage<NewsItemViewModel>
    {
        public NewsItemSelected(NewsItemViewModel viewModel) : base(viewModel)
        { }
    }
}