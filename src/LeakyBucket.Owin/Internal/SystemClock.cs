using System;

namespace LeakyBucket.Owin.Internal
{
    public interface ISystemClock
    {
        DateTime UtcNow { get; }
    }
    
    internal class SystemClock : ISystemClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}