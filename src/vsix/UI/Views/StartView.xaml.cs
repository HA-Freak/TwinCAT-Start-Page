namespace TwinCAT.XAE.StartPage.UI.Views
{
    using System.Windows;
    using System.Windows.Controls;

    using ViewModels;

    public partial class StartView : UserControl
    {
        // narrower than this per column, the news are minimized so recent and start items keep enough room
        private const double MinColumnWidth = 320;

        public StartView()
            => InitializeComponent();

        // the decision uses the width of all three columns, so minimizing the news doesn't flip it back
        private void OnColumnsSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.WidthChanged && DataContext is StartViewModel { NewsItems: { } newsItems })
                newsItems.IsTooNarrow = e.NewSize.Width / 3 < MinColumnWidth;
        }
    }
}
