using System;

namespace StartPageTcXaeShell64.Core.Services
{
    using Interfaces;

    public class DateTimeService : IDateTimeService
    {
        public DateTime Today
            => DateTime.Today.Date;

        public DateTime Now
            => DateTime.UtcNow;
    }
}