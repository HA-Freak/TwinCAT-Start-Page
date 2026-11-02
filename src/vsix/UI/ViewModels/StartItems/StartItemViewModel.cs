using System.Windows.Media;

using CommunityToolkit.Mvvm.Input;

using Microsoft.VisualStudio.Imaging.Interop;

namespace TwinCAT.XAE.StartPage.UI.ViewModels.StartItems
{
    using TwinCAT.XAE.StartPage.UI.Interfaces;

    public abstract partial class StartItemViewModel : ViewModelBase
    {
        public StartItemViewModel(IStartItemActionService actionService)
        {
            ActionService = actionService;
            ImageSize = 28;
            Margin = "5,3,0,0";
            IsEnabled = true;
        }

        public ImageMoniker Moniker { get; set; }

        // shown instead of Moniker when set, for icons that aren't in KnownMonikers
        public ImageSource Image { get; set; }

        public bool HasImage
            => Image is not null;

        public int ImageSize { get; set; }

        public string Margin { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsVisible { get; set; }

        // a one-line link with a small icon and no description, e.g. 'New Measurement Project...'
        public bool IsCompact { get; set; }

        // the last of a group of compact links: followed by the same gap as a regular item
        public bool IsGroupEnd { get; set; }

        // followed by its compact child links, so without the gap to them
        public bool HasChildren { get; set; }

        public bool IsEnabled { get; set; }

        public IStartItemActionService ActionService { get; }

        [RelayCommand]
        protected virtual void OnClick()
        { }
    }
}