namespace StartPageTcXaeShell64.UI.Models
{
    using System;
    using System.IO;

    using StartPageTcXaeShell64.UI.Enums;
    using StartPageTcXaeShell64.UI.ViewModels.RecentItems;

    using static StartPageTcXaeShell64.UI.Enums.PeriodTypes;
    using static StartPageTcXaeShell64.UI.Enums.RecentItemTypes;

    internal class RecentItem
    {
        public RecentItem()
        { }

        public string Key { get; set; }

        public RecentItemValue Value { get; set; }

        public RecentItemViewModel CreateViewModel(DateTime today, bool showExtension, bool showPath)
        {
            var path = Value.LocalProperties.FullPath;
            var date = Value.LastAccessed.Date;
            var name = showExtension
                ? Path.GetFileName(path)
                : Path.GetFileNameWithoutExtension(path);
            var pinned = Value.IsFavorite;
            var folder = showPath
                ? Path.GetDirectoryName(path)
                : "";
            var period = CalculatePeriodType(pinned, today, date);
            var type = path.CalculateRecentItemType();
            var moniker = type.ToImageMoniker();

            //YD: I shouldn't be manually newing up vew models (AddTransient<IRecentItemViewModel> in RegisterViewModels?)
            return new RecentItemViewModel
            {
                Name = name,
                Description = folder,
                Date = date,
                Path = path,
                Pinned = pinned,
                PeriodType = period,
                ItemType = type,
                Moniker = moniker
            };
        }
    }
}