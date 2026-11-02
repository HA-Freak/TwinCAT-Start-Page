using System;

using CommunityToolkit.Mvvm.Messaging;

namespace TwinCAT.XAE.StartPage.UI.Messages
{
    using DI;

    internal class MessengerManager
    {
        private static StartPageContainer _container;

        //---

        public static IMessenger Messenger
            => _container?.GetInstance<IMessenger>();

        //---

        public static void RegisterMessenger(StartPageContainer container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));

            _container.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        }
    }
}