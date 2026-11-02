namespace TwinCAT.XAE.StartPage.UI.ToolWindows
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    using Community.VisualStudio.Toolkit;

    using Microsoft.VisualStudio.Imaging;
    using Microsoft.VisualStudio.Shell;

    using TwinCAT.XAE.StartPage.UI.Views;

    public class BrowserWindow : BaseToolWindow<BrowserWindow>
    {
        public static BrowserView View { get; private set; }

        public override string GetTitle(int toolWindowId)
            => "Web Browser";

        public override Type PaneType
            => typeof(Pane);

        public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
        {
            View = new BrowserView();

            return Task.FromResult<FrameworkElement>(View);
        }

        public static async Task OpenAsync(string url, string title = null)
        {
            var pane = await ShowAsync();

            if (pane != null)
            {
                if (!string.IsNullOrEmpty(title))
                {
                    pane.Caption = title;
                }

                // keep the tab caption in sync with the page while browsing
                View.TitleChanged = documentTitle =>
                {
                    if (!string.IsNullOrEmpty(documentTitle))
                    {
                        pane.Caption = documentTitle;
                    }
                };
            }

            await View.NavigateAsync(url);
        }

        //---

        [Guid("14ae0cfc-470a-45f0-b95f-8f6eac61f152")]
        public class Pane : ToolWindowPane
        {
            public Pane()
            {
                Caption = "Web Browser";
                BitmapImageMoniker = KnownMonikers.WebURL;
            }
        }
    }
}
