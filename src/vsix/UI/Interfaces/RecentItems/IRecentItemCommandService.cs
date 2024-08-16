namespace StartPageTcXaeShell64.UI.Interfaces.RecentItems
{
    using System;
    using System.Collections.Generic;

    using StartPageTcXaeShell64.UI.ViewModels;

    public interface IRecentItemCommandService
    {
        List<CommandViewModel> GetCommands(Action refresh, Action showSettings);

        List<ContextCommandViewModel> GetContextCommands(
            Func<bool> canPin, Action pin,
            Func<bool> canUnpin, Action unpin,
            Func<bool> canRemove, Action remove,
            Func<bool> canCopyPath, Action copyPath,
            Func<bool> canEditPath, Action editPath,
            Func<bool> canOpenInVS, Action openInVS);
    }
}