namespace TwinCAT.XAE.StartPage.UI.Models
{
    using System;
    using System.IO;

    using TwinCAT.XAE.StartPage.UI.Enums;
    using TwinCAT.XAE.StartPage.UI.ViewModels.RecentItems;

    using static TwinCAT.XAE.StartPage.UI.Enums.PeriodTypes;
    using static TwinCAT.XAE.StartPage.UI.Enums.RecentItemTypes;

    internal class RecentItem
    {
        public RecentItem()
        { }

        public string Key { get; set; }

        public RecentItemValue Value { get; set; }

        public RecentItemViewModel CreateViewModel(DateTime today, bool showExtension, bool showPath)
        {
            var path = Value.LocalProperties.FullPath;
            // LastAccessed is stored in UTC; group by the local calendar day
            var date = Value.LastAccessed.ToLocalTime().Date;
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