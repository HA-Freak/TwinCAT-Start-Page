namespace TwinCAT.XAE.StartPage.UI.Interfaces.NewsItems
{
    using System;
    using System.Collections.Generic;

    using TwinCAT.XAE.StartPage.UI.ViewModels;

    public interface INewsItemCommandService
    {
        List<CommandViewModel> GetCommands(Action refresh, Action openSettings);
    }
}