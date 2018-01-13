using System;
using LeakyBucket.Owin.Identifiers;
using LeakyBucket.Owin.Store;
using Microsoft.Owin;

namespace LeakyBucket.Owin
{
    public class LeakyBucketConfiguration
    {
        private const int DefaultMaxNumberOfRequests = 10;
        
        public IRequestStore RequestStore { get; set; } = new DefaultRequestStore(DefaultMaxNumberOfRequests);
        public TimeSpan RefreshRate { get; set; } = TimeSpan.FromSeconds(1);
        public int MaxNumberOfRequests { get; set; } = DefaultMaxNumberOfRequests;
        public Func<IOwinContext, IClientIdentifier> ClientIdentifierFunc { get; set; } = DefaultClientIdentifierFunc();
        
        private static Func<IOwinContext, IClientIdentifier> DefaultClientIdentifierFunc()
        {
            return ctx => new DefaultClientIdentifier(ctx.Request.Headers.Get("User-Agent"));
        }
    }
}