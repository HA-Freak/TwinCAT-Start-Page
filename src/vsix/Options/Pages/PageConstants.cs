namespace TwinCAT.XAE.StartPage.Options.Pages
{
    using static Core.Strings.Constants;

    public static class PageConstants
    {
        //YD: change all of these constant declarations to internal

        public const string H1 = "1." + Space;
        public const string H2 = "2." + Space;
        public const string H3 = "3." + Space;
        public const string Feature = "Feature";
        public const string Features = "Features";
        public const string FeatureSet = "Feature Set";
        public const string Enable = "Enable";
        public const string Enabled = "Enabled";
        public const string EnablesDisables = "Enables/disables";
        public const string EnablesDisablesAll = EnablesDisables + Space + "ALL";
        public const string Settings = "Settings";
        public const string Appearance = "Appearance";
        public const string Behavior = "Behavior";

        // package

        public const string PackageName = Vsix.Name;
        public const string PackageFeatureSet = PackageName + Space + FeatureSet;

        // Tools > Options node; "TwinCAT\XAE Environment" is registered by TwinCAT XAE itself
        public const string OptionsCategory = @"TwinCAT\XAE Environment\Start Page";

        // TwinCAT.XAE.StartPage

        public const string EnableStartPageDispayName = "Enable '" + PackageName + "'";
        public const string EnableStartPageDescription = "Allows the whole set of '" + PackageName + "' features to be turned off together";
        internal const bool EnableStartPageDefault = true;

        public const string EnableOptionsMenuItemDispayName = "Enable '" + PackageName + "' Options";
        public const string EnableOptionsMenuItemDescription = "Determines if a menu item to open the options page is added to the Tools | Options menu";
        internal const bool EnableStartPageOptionsDefault = true;

        public const string PackageVersion = "Version Number";

        public const string MaxWidthDisplayName = "Max Width";
        public const string MaxWidthDescription = "Sets the max width of contents of the TwinCAT.XAE.StartPage window";
        internal const int MaxWidthDefault = 1280;

        // General

        public const string ShowStartTabTitleDisplayName = "Show Title";
        public const string ShowStartTabTitleDescription = "Adds a title above the panes";
        internal const bool ShowStartTabTitleDefault = true;

        public const string StartTabTitleTextDisplayName = "Title Text";
        public const string StartTabTitleTextDescription = "Sets the text of the title (a welcome message, or a company name etc)";
        internal const string StartTabTitleDefault = "TwinCAT 3";

        public const string ShowOnStartupDisplayName = "Show Window on Startup";
        public const string ShowOnStartupDescription = "Shows the 'TwinCAT.XAE.StartPage' window when the shell starts without a solution";
        internal const bool ShowOnStartupDefault = true;

        public const string HideOnSolutionOpenDisplayName = "Hide Window on Solution Load";
        public const string HideOnSolutionOpenDescription = "Hides the 'TwinCAT.XAE.StartPage' window when a solution is loaded";
        internal const bool HideOnSolutionOpenDefault = true;

        public const string RestoreOnSolutionCloseDisplayName = "Show Window on Solution Close";
        public const string RestoreOnSolutionCloseDescription = "Shows the 'TwinCAT.XAE.StartPage' window when a solution is closed";
        internal const bool RestoreOnSolutionCloseDefault = true;

        // Recent Items

        public const string RecentItemsToDisplayDisplayName = "Items To Display";
        public const string RecentItemsToDisplayDescription = "Sets the number of items to display in the 'Recent Items' list";
        internal const int RecentItemsToDisplayDefault = 50;

        public const string ShowFileExtensionsDisplayName = "Show File Extensions";
        public const string ShowFileExtensionsDescription = "Sets the visibility of project/solution extensions in the 'Recent Items' list";
        internal const bool ShowFileExtensionsDefault = false;

        public const string ShowFilePathsDisplayName = "Show File Paths";
        public const string ShowFilePathsDescription = "Sets the visibility of project/solution paths in the 'Recent Items' list";
        internal const bool ShowFilePathsDefault = true;

        public const string ShowCsProjFilesDisplayName = "Show csproj files";
        public const string ShowCsProjFilesDescription = "Sets the visibility of C# project files in the 'Recent Items' list";
        internal const bool ShowCsProjFilesDefault = true;

        public const string DisplayRecentItemsName = "Display 'Recent Items' list";
        public const string DisplayRecentItemsDescription = "Sets the visibility of the 'Recent Items' list";
        internal const bool DisplayRecentItemsDefault = true;

        // Start Items

        public const string CloneRepositoryDelayDisplayName = "'Clone a Repository' Delay";
        public const string CloneRepositoryDelayDescription = "Sets the number of seconds to delay before refreshing the 'Recent Items' list";
        public const int CloneRepositoryDelayDefault = 5;

        public const string OpenFolderDelayDisplayName = "'Open a Local Folder' Delay";
        public const string OpenFolderDelayDescription = "Sets the number of seconds to delay before refreshing the 'Recent Items' list";
        public const int OpenFolderDelayDefault = 5;

        public const string OpenProjectDelayDisplayName = "'Open Project/Solution' Delay";
        public const string OpenProjectDelayDescription = "Sets the number of seconds to delay before refreshing the 'Recent Items' list";
        public const int OpenProjectDelayDefault = 5;

        public const string CreateProjectDelayDisplayName = "'Create a New Project' Delay";
        public const string CreateProjectDelayDescription = "Sets the number of seconds to delay before refreshing the 'Recent Items' list";
        public const int CreateProjectDelayDefault = 5;

        public const string TwinCatProjects = "TwinCAT Projects";

        public const string TwinCatProjectLocationDisplayName = "Default Location";
        public const string TwinCatProjectLocationDescription = "The folder 'Create a TwinCAT project' suggests for new projects. Environment variables such as %USERPROFILE% are allowed. Defaults to the projects location of the shell - clear the field to go back to it";
        public const string TwinCatProjectLocationDefault = "";

        // News Items

        public const string ClearListBeforeRefreshDisplayName = "Clear List Before Refresh";
        public const string ClearListBeforeRefreshDescription = "Clears the list to give a visual indication that the list has been refreshed";
        internal const bool ClearListBeforeRefreshDefault = true;

        public const string NewsItemsFeedUrlDisplayName = "Items Feed Url";
        public const string NewsItemsFeedUrlDescription = "Sets the url of the 'Developer News' feed";
        public const string NewsItemsFeedUrlDefault = "http://www.beckhoff.com/english/rss/beckhoff-twincat-rss-feed.xml";

        public const string NewsItemsToDisplayDisplayName = "Items To Display";
        public const string NewsItemsToDisplayDescription = "Sets the number of items to display in the 'News Items' list";
        internal const int NewsItemsToDisplayDefault = 500;

        public const string OpenLinksInVsDisplayName = "Open Links in VS";
        public const string OpenLinksInVsDescription = "Determines if news links are opened in VS or in the default browser";
        internal const bool OpenLinksInVsDefault = true;

        public const string DisplayNewsItemsName = "Display 'News Items' list";
        public const string DisplayNewsItemsDescription = "Sets the visibility of the 'News Items' list";
        internal const bool DisplayNewsItemsDefault = true;
    }
}