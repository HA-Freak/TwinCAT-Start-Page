using CommunityToolkit.Mvvm.Messaging.Messages;

namespace StartPageTcXaeShell64.UI.Messages
{
    using ViewModels.RecentItems;

    public sealed class RecentItemSelected : ValueChangedMessage<RecentItemViewModel>
    {
        public RecentItemSelected(RecentItemViewModel viewModel) : base(viewModel)
        { }
    }
}