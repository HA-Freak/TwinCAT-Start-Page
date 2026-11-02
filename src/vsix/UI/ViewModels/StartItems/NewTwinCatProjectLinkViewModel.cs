using System.Windows.Media;

using Microsoft.VisualStudio.Imaging;

namespace TwinCAT.XAE.StartPage.UI.ViewModels.StartItems
{
    using Interfaces;

    // a compact link like 'New Measurement Project...' that opens the Create TwinCAT project dialog in its category
    public class NewTwinCatProjectLinkViewModel : StartItemViewModel
    {
        private readonly string _category;

        public NewTwinCatProjectLinkViewModel(IStartItemActionService actionService, string name, string category, ImageSource icon)
            : base(actionService)
        {
            _category = category;

            Moniker = KnownMonikers.AddDocument;
            Image = icon;
            Name = name;
            Description = "";
            IsCompact = true;
            ImageSize = 20;
            Margin = "5,0,0,0";
        }

        // the start page is hidden by EventManager when the new solution opens,
        // so cancelling the dialog leaves the start page untouched
        protected override void OnClick()
            => ActionService.CreateTwinCatProject(_category);
    }
}
