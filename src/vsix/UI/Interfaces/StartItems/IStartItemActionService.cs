namespace StartPageTcXaeShell64.UI.Interfaces
{
    public interface IStartItemActionService
    {
        bool CloneRepository();

        bool CreateNewProject();

        bool OpenFolder();

        bool OpenProject();

        bool RestartVisualStudio(bool confirm, bool elevated);

        bool OpenWebPage(string url, bool openInVS);
    }
}