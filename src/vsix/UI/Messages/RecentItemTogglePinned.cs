using CommunityToolkit.Mvvm.Messaging.Messages;

namespace TwinCAT.XAE.StartPage.UI.Messages
{
    using ViewModels.RecentItems;

    public sealed class RecentItemTogglePinned : ValueChangedMessage<RecentItemViewModel>
    {
        public RecentItemTogglePinned(RecentItemViewModel viewModel) : base(viewModel)
        { }
    }
}