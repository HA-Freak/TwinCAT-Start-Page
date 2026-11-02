using CommunityToolkit.Mvvm.Messaging.Messages;

namespace TwinCAT.XAE.StartPage.UI.Messages
{
    using ViewModels.RecentItems;

    public sealed class RecentItemCopyPath : ValueChangedMessage<RecentItemViewModel>
    {
        public RecentItemCopyPath(RecentItemViewModel viewModel) : base(viewModel)
        { }
    }
}