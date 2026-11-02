using System;

using Community.VisualStudio.Toolkit;

using CommunityToolkit.Mvvm.Messaging;

namespace TwinCAT.XAE.StartPage.UI.Events
{
    using Core;
    using Core.Interfaces;

    using DI;

    using Messages;

    using Options.Pages;

    using Services;

    using ToolWindows;

    internal class EventManager
    {
        private static readonly IDialogService _dialogService;
        private static readonly IAsyncMethodService _methodService;
        private static readonly IMessenger _messenger;

        //---

        static EventManager()
        {
            _dialogService = ServiceManager.DialogService;
            _methodService = ServiceManager.AsyncMethodService;
            _messenger = MessengerManager.Messenger;
        }

        //---

        internal static void RegisterEvents(StartPageContainer container)
        {
            VS.Events.SolutionEvents.OnBeforeOpenSolution += OnBeforeOpenSolution;
            VS.Events.SolutionEvents.OnAfterOpenSolution += OnAfterOpenSolution;
            VS.Events.SolutionEvents.OnAfterOpenFolder += OnAfterOpenFolder;
            VS.Events.SolutionEvents.OnAfterCloseSolution += OnAfterCloseSolution;
        }

        internal static void DeregisterEvents(StartPageContainer container)
        {
            VS.Events.SolutionEvents.OnAfterCloseSolution -= OnAfterCloseSolution;
            VS.Events.SolutionEvents.OnAfterOpenFolder -= OnAfterOpenFolder;
            VS.Events.SolutionEvents.OnAfterOpenSolution -= OnAfterOpenSolution;
            VS.Events.SolutionEvents.OnBeforeOpenSolution -= OnBeforeOpenSolution;
        }

        //---

        // fires when an existing .sln is loaded from disk
        private static void OnBeforeOpenSolution(string obj)
        {
            if (GeneralOptions.Instance.HideOnSolutionOpen == true)
            {
                HideWindow();
            }
        }

        // fires for newly created solutions too (e.g. 'Create a new project'),
        // which OnBeforeOpenSolution does not
        private static void OnAfterOpenSolution(Solution solution)
        {
            if (GeneralOptions.Instance.HideOnSolutionOpen == true)
            {
                HideWindow();
            }
        }

        private static void OnAfterOpenFolder(string obj)
        {
            if (GeneralOptions.Instance.HideOnSolutionOpen == true)
            {
                HideWindow();
            }
        }

        private static void HideWindow()
        {
            _methodService.Run(
                async () =>
                {
                    var result = false;

                    try
                    {
                        var mainWindow = await VS.Windows.FindWindowAsync(PackageGuids.StartPageWindow);

                        if (mainWindow is null)
                        {
                            return true; // nothing to hide
                        }

                        await mainWindow.CloseFrameAsync(FrameCloseOption.NoSave);

                        result = true;
                    }
                    catch (Exception ex)
                    {
                        result = false;
                        _dialogService.ShowException(ex);
                    }

                    return result;
                }
            );
        }

        private static void OnAfterCloseSolution()
        {
            if (GeneralOptions.Instance.RestoreOnSolutionClose == true)
            {
                ShowWindow();
            }

            RefreshRecentItems();

            //---

            static void ShowWindow()
            {
                _methodService.Run(
                    async () =>
                    {
                        try
                        {
                            // creates the tool window if it doesn't exist yet (e.g. the tab was closed)
                            await MainWindow.ShowAsync();

                            return true;
                        }
                        catch (Exception ex)
                        {
                            _dialogService.ShowException(ex);

                            return false;
                        }
                    }
                );
            }

            static void RefreshRecentItems()
                => _messenger.Send<RefreshRecentItems>();
        }
    }
}