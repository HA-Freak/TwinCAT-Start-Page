namespace TwinCAT.XAE.StartPage.UI.Services.StartItems
{
    using System.Collections.Generic;
    using System.Linq;

    using CommunityToolkit.Mvvm.Messaging;

    using Interfaces;
    using Interfaces.StartItems;

    using Other;

    using TwinCAT.XAE.StartPage.Core;

    using ViewModels.StartItems;

    internal class StartItemDataService : ServiceBase, IStartItemDataService
    {
        // like TwinCAT's own start page: one link per installed product, each opening the
        // Create TwinCAT project dialog in that product's category
        private static readonly (string Name, string Category)[] NewProjectLinks =
        {
            ("New TwinCAT Project...", null), // 'TwinCAT Projects' - see FindLinkTemplate
            ("New Measurement Project...", TwinCatProject.MeasurementCategory),
            ("New Connectivity Project...", TwinCatProject.ConnectivityCategory),
            ("New TwinCAT HMI Project...", TwinCatProject.HmiCategory),
        };

        private readonly CloneRepositoryViewModel _cloneRepository;
        private readonly OpenFolderViewModel _openFolder;
        private readonly OpenProjectViewModel _openProject;
        private readonly CreateProjectViewModel _createProject;
        private readonly RestartNormalViewModel _restartNormal;
        private readonly RestartElevatedViewModel _restartElevated;
        private readonly IStartItemActionService _actionService;

        public StartItemDataService(
            CloneRepositoryViewModel cloneRepository,
            OpenFolderViewModel openFolder,
            OpenProjectViewModel openProjext,
            CreateProjectViewModel createProject,
            RestartNormalViewModel restartNormal,
            RestartElevatedViewModel restartElevated,
            IStartItemActionService actionService,
            IAsyncMethodService methodService, IMessenger messenger)
            : base(methodService, messenger)
        {
            _cloneRepository = cloneRepository;
            _openFolder = openFolder;
            _openProject = openProjext;
            _createProject = createProject;
            _restartNormal = restartNormal;
            _restartElevated = restartElevated;
            _actionService = actionService;
        }

        public List<StartItemViewModel> GetItems()
        {
            var items = new List<StartItemViewModel>
            {
                _cloneRepository,
                _openFolder,
                _openProject,
                _createProject,
                _restartNormal,
                _restartElevated // icon size has been tweaked in viewmodel
            };

            var links = GetNewProjectLinks().ToList();

            // the links are indented children of 'Create a new project'
            if (links.Count > 0)
            {
                _createProject.HasChildren = true;
                links[links.Count - 1].IsGroupEnd = true;
            }

            items.InsertRange(items.IndexOf(_createProject) + 1, links);

            return items;
        }

        // only the products whose templates are installed get a link
        private IEnumerable<StartItemViewModel> GetNewProjectLinks()
        {
            var templates = TwinCatProject.FindTemplates();

            foreach (var (name, category) in NewProjectLinks)
            {
                var template = FindLinkTemplate(templates, category);

                if (template != null)
                    yield return new NewTwinCatProjectLinkViewModel(_actionService, name, template.Category, template.Icon);
            }
        }

        private static TwinCatProjectTemplate FindLinkTemplate(IReadOnlyList<TwinCatProjectTemplate> templates, string category)
            => (category is null)
                ? templates.FirstOrDefault(t => t.CategoryOrder == TwinCatProject.TwinCatProjectsCategoryOrder)
                : templates.FirstOrDefault(t => t.Category == category);
    }
}
