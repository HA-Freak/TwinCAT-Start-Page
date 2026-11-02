namespace TwinCAT.XAE.StartPage.DI
{
    using UI.Services;
    using UI.ViewModels;

    internal class StartPageContainer : MicrosoftDependencyInjectionContainer
    {
        private static StartPageContainer _instance;

        //---

        public static StartPageContainer Instance
            => _instance ??= new StartPageContainer();

        //---

        public T GetService<T>()
            where T : ServiceBase
            => GetInstance<T>();

        public T GetViewModel<T>()
            where T : ViewModelBase
            => GetInstance<T>();
    }
}
