using CommunityToolkit.Mvvm.Messaging.Messages;

namespace TwinCAT.XAE.StartPage.UI.Messages
{
    using ViewModels.RecentItems;

    public sealed class RecentItemEditPath : ValueChangedMessage<RecentItemViewModel>
    {
        public RecentItemEditPath(RecentItemViewModel viewModel) : base(viewModel)
        { }
    }
}