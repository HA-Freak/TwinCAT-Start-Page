using System;

namespace StartPageTcXaeShell64.Core.Interfaces
{
    public interface IDateTimeService : ISimpleService
    {
        DateTime Today { get; }

        DateTime Now { get; }
    }
}