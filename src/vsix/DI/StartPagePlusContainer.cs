namespace StartPageTcXaeShell64.DI
{
    using UI.Services;
    using UI.ViewModels;

    internal class StartPageTcXaeShell64Container : MicrosoftDependencyInjectionContainer
    {
        private static StartPageTcXaeShell64Container _instance;

        //---

        public static StartPageTcXaeShell64Container Instance
            => _instance ??= new StartPageTcXaeShell64Container();

        //---

        public T GetService<T>()
            where T : ServiceBase
        {
            try
            {
                var service = GetInstance<T>();

                return service;

            }
            catch (System.Exception)
            {
                throw;
            }
        }

        public T GetViewModel<T>()
            where T : ViewModelBase
        {
            try
            {
                var viewModel = GetInstance<T>();

                return viewModel;

            }
            catch (System.Exception)
            {
                throw;
            }
        }
    }
}