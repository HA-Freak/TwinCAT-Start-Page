namespace TwinCAT.XAE.StartPage.Commands
{
    using System;

    using Community.VisualStudio.Toolkit;

    using Microsoft.VisualStudio.Shell;

    using TwinCAT.XAE.StartPage.UI.ToolWindows;

    //using TwinCAT.XAE.StartPage.Options.Models;
    //using TwinCAT.XAE.StartPage.UI.ToolWindows;

    using Task = System.Threading.Tasks.Task;

    [Command(PackageIds.FileWindow)]
    internal sealed class ViewWindow : BaseCommand<ViewWindow>
    {
        //protected override void BeforeQueryStatus(EventArgs e)
        //    => Command.Enabled = GeneralOptions.Instance.EnableStartPageOptions;

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