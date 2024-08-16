using CommunityToolkit.Mvvm.Messaging.Messages;

namespace StartPageTcXaeShell64.UI.Messages
{
    using ViewModels.RecentItems;

    public sealed class RecentItemCopyPath : ValueChangedMessage<RecentItemViewModel>
    {
        public RecentItemCopyPath(RecentItemViewModel viewModel) : base(viewModel)
        { }
    }
}