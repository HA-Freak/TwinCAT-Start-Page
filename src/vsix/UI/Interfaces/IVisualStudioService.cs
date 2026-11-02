using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell;

namespace TwinCAT.XAE.StartPage.UI.Interfaces
{
    public interface IVisualStudioService
    {
        bool ExecuteCommand(string action, string args = "", int delay = 0);

        bool OpenWebPage(string url, bool internalBrowser, string title = null);

        bool CloneRepository(int delay);

        //---

        bool OpenFolder(int delay);

        bool OpenFolder(string path, int delay);

        //---

        bool OpenProject(int delay);

        bool OpenProject(string path, int delay);

        //---

        bool CreateNewProject(int delay);

        bool CreateTwinCatProject(int delay, string category = null);

        bool Restart(bool confirm = true, bool elevated = false);

        Task<bool> OpenInNewInstanceAsync(string path);

        bool ShowOptions<T>() where T : DialogPage;
    }
}