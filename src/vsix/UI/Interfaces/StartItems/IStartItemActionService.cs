namespace TwinCAT.XAE.StartPage.UI.Interfaces
{
    public interface IStartItemActionService
    {
        bool CloneRepository();

        bool CreateNewProject();

        bool CreateTwinCatProject(string category = null);

        bool OpenFolder();

        bool OpenProject();

        bool RestartVisualStudio(bool confirm, bool elevated);

        bool OpenWebPage(string url, bool openInVS, string title = null);
    }
}