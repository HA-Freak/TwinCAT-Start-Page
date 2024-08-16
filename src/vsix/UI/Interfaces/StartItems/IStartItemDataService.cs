using System.Collections.Generic;

namespace StartPageTcXaeShell64.UI.Interfaces.StartItems
{
    using ViewModels.StartItems;

    public interface IStartItemDataService
    {
        List<StartItemViewModel> GetItems();
    }
}