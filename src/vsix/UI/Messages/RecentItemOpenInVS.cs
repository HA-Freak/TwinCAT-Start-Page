using CommunityToolkit.Mvvm.Messaging.Messages;

namespace StartPageTcXaeShell64.UI.Messages
{
    using ViewModels.RecentItems;

    public sealed class RecentItemOpenInVS : ValueChangedMessage<RecentItemViewModel>
    {
        public RecentItemOpenInVS(RecentItemViewModel viewModel) : base(viewModel)
        { }
    }
}