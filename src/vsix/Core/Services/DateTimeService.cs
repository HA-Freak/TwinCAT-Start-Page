using System;

namespace TwinCAT.XAE.StartPage.Core.Services
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