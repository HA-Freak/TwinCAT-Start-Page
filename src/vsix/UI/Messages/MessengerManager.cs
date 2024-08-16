using System;

using CommunityToolkit.Mvvm.Messaging;

namespace StartPageTcXaeShell64.UI.Messages
{
    using DI;

    internal class MessengerManager
    {
        private static StartPageTcXaeShell64Container _container;

        //---

        public static IMessenger Messenger
            => _container?.GetInstance<IMessenger>();

        //---

        public static void RegisterMessenger(StartPageTcXaeShell64Container container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));

            _container.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        }
    }
}