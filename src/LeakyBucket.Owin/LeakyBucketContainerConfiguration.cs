using System;

namespace LeakyBucket.Owin
{
    internal class LeakyBucketContainerConfiguration
    {
        public TimeSpan RefreshRate { get; set; } = TimeSpan.FromSeconds(1);
        public int Limit { get; set; }
    }
}