using CommunityToolkit.Mvvm.Messaging.Messages;

namespace TwinCAT.XAE.StartPage.UI.Messages
{
    using ViewModels.RecentItems;

    public sealed class RecentItemRemove : ValueChangedMessage<RecentItemViewModel>
    {
        public RecentItemRemove(RecentItemViewModel viewModel) : base(viewModel)
        { }
    }
}