using System;
using System.Runtime.InteropServices;
using System.Threading;

using Community.VisualStudio.Toolkit;

using Microsoft.VisualStudio.Shell;

namespace TwinCAT.XAE.StartPage
{
    using DI;
    using Options.Pages;

    using UI.Events;
    using UI.Messages;
    using UI.Services;
    using UI.ToolWindows;
    using UI.ViewModels;

    using static Vsix;

    using Task = System.Threading.Tasks.Task;

    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration(Name, Description, Vsix.Version)]
    [Guid(PackageGuids.StartPageString)]

    [ProvideAutoLoad(Microsoft.VisualStudio.VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)] //SRS

    [ProvideMenuResource("Menus.ctmenu", 1)]

    [ProvideToolWindow(typeof(MainWindow.Pane), Style = VsDockStyle.Tabbed, Window = WindowGuids.DocumentWell, MultiInstances = false, Transient = true)]
    [ProvideToolWindow(typeof(BrowserWindow.Pane), Style = VsDockStyle.Tabbed, Window = WindowGuids.DocumentWell, MultiInstances = false, Transient = true)]

    [ProvideOptionPage(typeof(OptionsProvider.General), PageConstants.OptionsCategory, GeneralOptions.Category, 0, 0, true)]
    [ProvideProfile(typeof(OptionsProvider.General), PageConstants.OptionsCategory, GeneralOptions.Category, 0, 0, true)]

    [ProvideOptionPage(typeof(OptionsProvider.RecentItems), PageConstants.OptionsCategory, RecentItemsOptions.Category, 0, 0, true)]
    [ProvideProfile(typeof(OptionsProvider.RecentItems), PageConstants.OptionsCategory, RecentItemsOptions.Category, 0, 0, true)]

    [ProvideOptionPage(typeof(OptionsProvider.StartItems), PageConstants.OptionsCategory, StartItemsOptions.Category, 0, 0, true)]
    [ProvideProfile(typeof(OptionsProvider.StartItems), PageConstants.OptionsCategory, StartItemsOptions.Category, 0, 0, true)]

    [ProvideOptionPage(typeof(OptionsProvider.NewsItems), PageConstants.OptionsCategory, NewsItemsOptions.Category, 0, 0, true)]
    [ProvideProfile(typeof(OptionsProvider.NewsItems), PageConstants.OptionsCategory, NewsItemsOptions.Category, 0, 0, true)]

    public sealed class StartPagePackage : ToolkitPackage
    {
        private readonly StartPageContainer _container;

        public StartPagePackage() : base()
            => _container = StartPageContainer.Instance;

        //YD: move XAML styles in situ where possible to help debugging experience, or does that work already?

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            await base.InitializeAsync(cancellationToken, progress);
            await this.RegisterCommandsAsync();

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            ServiceManager.RegisterServices(_container);
            ViewModelManager.RegisterViewModels(_container);
            MessengerManager.RegisterMessenger(_container);
            EventManager.RegisterEvents(_container);

            this.RegisterToolWindows();

            await VS.StatusBar.ShowMessageAsync("StartPage");// SRS

            // the package auto-loads in the NoSolution context, so this runs when the
            // shell starts without a solution - show the start page unless disabled.
            // StartOnIdle defers the window until the shell is fully loaded; showing
            // it any earlier deadlocks tool window creation against package
            // initialization ("Working on it...")
            if (GeneralOptions.Instance.ShowOnStartup)
            {
                ThreadHelper.JoinableTaskFactory.StartOnIdle(
                    () => MainWindow.ShowAsync());
            }
        }

        protected override object GetService(Type serviceType)
        {
            if (_container?.IsRegistered(serviceType) ?? false) // try in the container first
            {
                return _container?.GetInstance(serviceType); // the container gets the requested service
            }

            return base.GetService(serviceType); // otherwise let VS handle getting the requested service
        }
    }
}