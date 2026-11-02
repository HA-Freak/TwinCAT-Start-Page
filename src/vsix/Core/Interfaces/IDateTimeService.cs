using System;

namespace TwinCAT.XAE.StartPage.Core.Interfaces
{
    public interface IDateTimeService : ISimpleService
    {
        DateTime Today { get; }

        DateTime Now { get; }
    }
}