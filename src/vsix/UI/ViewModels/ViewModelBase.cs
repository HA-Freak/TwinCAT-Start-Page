using System;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace StartPageTcXaeShell64.UI.ViewModels
{
    using Interfaces;

    using StartPageTcXaeShell64.Core;
    using StartPageTcXaeShell64.UI.Services;

    public class ViewModelBase : ObservableRecipient, IViewModel
    {
        private readonly IAsyncMethodService _methodService;

        //---

        protected ViewModelBase()
        {
            _methodService = ServiceManager.AsyncMethodService ?? throw new ArgumentNullException(nameof(ServiceManager.AsyncMethodService));
        }

        //---

        protected void ListenFor<TMessage>(object recipient, MessageHandler<object, TMessage> action)
            where TMessage : class //, new()
            => Messenger.Register(recipient, action);

        public void Run(Func<Task> asyncMethod)
            => _methodService.Run(asyncMethod);

        public bool Run(Func<Task<bool>> asyncMethod)
            => _methodService.Run(asyncMethod);

        public bool? Run(Func<Task<bool?>> asyncMethod)
            => _methodService.Run(asyncMethod);
    }
}