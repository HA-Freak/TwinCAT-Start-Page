namespace StartPageTcXaeShell64.Commands
{
    using System;

    using Community.VisualStudio.Toolkit;

    using Microsoft.VisualStudio.Shell;

    using StartPageTcXaeShell64.UI.ToolWindows;

    //using StartPageTcXaeShell64.Options.Models;
    //using StartPageTcXaeShell64.UI.ToolWindows;

    using Task = System.Threading.Tasks.Task;

    [Command(PackageIds.ViewWindow)]
    internal sealed class ViewWindow : BaseCommand<ViewWindow>
    {
        //protected override void BeforeQueryStatus(EventArgs e)
        //    => Command.Enabled = GeneralOptions.Instance.EnableStartPageTcXaeShell64Options;

        protected async override Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            try
            {
                await MainWindow.ShowAsync();
            }
            catch (Exception ex)
            {
                await VS.MessageBox.ShowErrorAsync(ex.Message);
            }
        }
    }
}