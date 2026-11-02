using System.Collections.Generic;

namespace TwinCAT.XAE.StartPage.UI.Interfaces.StartItems
{
    using ViewModels.StartItems;

    public interface IStartItemDataService
    {
        List<StartItemViewModel> GetItems();
    }
}