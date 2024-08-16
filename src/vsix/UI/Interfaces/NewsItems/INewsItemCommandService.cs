namespace StartPageTcXaeShell64.UI.Interfaces.NewsItems
{
    using System;
    using System.Collections.Generic;

    using StartPageTcXaeShell64.UI.ViewModels;

    public interface INewsItemCommandService
    {
        List<CommandViewModel> GetCommands(Action refresh, Action openSettings);
    }
}