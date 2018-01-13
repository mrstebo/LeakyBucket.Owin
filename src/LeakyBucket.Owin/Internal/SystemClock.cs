using System;

namespace LeakyBucket.Owin.Internal
{
    internal interface ISystemClock
    {
        DateTime UtcNow { get; }
    }
    
    internal class SystemClock : ISystemClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}