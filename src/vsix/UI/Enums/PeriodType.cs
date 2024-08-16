namespace StartPageTcXaeShell64.UI.Enums
{
    using System.ComponentModel;

    //https://stackoverflow.com/questions/424366/string-representation-of-an-enum/8353191#8353191

    public enum PeriodType
    {

        Pinned = 0,
        Today = 1,
        Yesterday = 2,
        [Description("This week")]
        ThisWeek = 3,
        [Description("This month")]
        ThisMonth = 4,
        [Description("Last month")]
        LastMonth = 5,
        Older = 5
    }
}