namespace TwinCAT.XAE.StartPage.UI.Views
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Microsoft.Web.WebView2.Core;

    public partial class BrowserView : UserControl
    {
        private static CoreWebView2Environment _environment;
        private static bool _loaderFolderSet;

        private bool _initialized;

        public BrowserView()
        {
            InitializeComponent();
        }

        // set by BrowserWindow to keep the tool window caption in sync with the page
        public Action<string> TitleChanged { get; set; }

        // TcXaeShell binding-redirects WebView2 to its own copy in PrivateAssemblies, which
        // has no WebView2Loader.dll next to it; must run before any other WebView2 call
        public static void UseBundledLoader()
        {
            if (_loaderFolderSet)
                return;

            _loaderFolderSet = true;

            try
            {
                var folder = Path.GetDirectoryName(typeof(BrowserView).Assembly.Location);

                if (File.Exists(Path.Combine(folder, "WebView2Loader.dll")))
                {
                    CoreWebView2Environment.SetLoaderDllFolderPath(folder);
                }
            }
            catch (Exception)
            {
                // the host already loaded a loader - WebView2 keeps using that one
            }
        }

        public async Task NavigateAsync(string url)
        {
            if (_environment is null)
            {
                // the extension folder is read-only, so WebView2 needs an explicit user data folder
                var userDataFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "TwinCAT.XAE.StartPage", "WebView2");

                _environment = await CoreWebView2Environment.CreateAsync(userDataFolder: userDataFolder);
            }

            await Browser.EnsureCoreWebView2Async(_environment);

            if (!_initialized)
            {
                _initialized = true;

                // keep links that would open a new window inside this view
                Browser.CoreWebView2.NewWindowRequested += (_, e) =>
                {
                    e.Handled = true;
                    Browser.CoreWebView2.Navigate(e.Uri);
                };

                // keep the address bar in sync with the current page
                Browser.CoreWebView2.SourceChanged += (_, __)
                    => AddressBox.Text = Browser.CoreWebView2.Source;

                Browser.CoreWebView2.DocumentTitleChanged += (_, __)
                    => TitleChanged?.Invoke(Browser.CoreWebView2.DocumentTitle);
            }

            AddressBox.Text = url;
            Browser.CoreWebView2.Navigate(url);
        }

        private void OnAddressBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            e.Handled = true;

            var url = AddressBox.Text?.Trim();

            if (string.IsNullOrEmpty(url))
                return;

            if (!url.Contains("://"))
                url = "https://" + url;

            try
            {
                Browser.CoreWebView2?.Navigate(url);
            }
            catch (ArgumentException)
            {
                // invalid url typed - leave the current page as is
            }
        }
    }
}
