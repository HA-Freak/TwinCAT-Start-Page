using CommunityToolkit.Mvvm.Messaging;

namespace TwinCAT.XAE.StartPage.UI.Services
{
    using Core.Interfaces;

    using TwinCAT.XAE.StartPage.Core;

    using UI.Interfaces;

    using ViewModels.NewsItems;

    internal class NewsItemActionService : ServiceBase, INewsItemActionService
    {
        public NewsItemActionService(IVisualStudioService vsService, IAsyncMethodService methodService, IMessenger messenger)
            : base(methodService, messenger)
            => VisualStudioService = vsService;

        public IVisualStudioService VisualStudioService { get; }

        public void DoAction(NewsItemViewModel currentViewModel, bool internalBrowser = true)
        {
            var url = currentViewModel.Link;

            VisualStudioService.OpenWebPage(url, internalBrowser, currentViewModel.Title);
        }
    }
}