using System;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace LeakyBucket.Owin
{
    public class LeakyBucketMiddleware : OwinMiddleware
    {
        private readonly LeakyBucketConfiguration _configuration;

        public LeakyBucketMiddleware(OwinMiddleware next, LeakyBucketConfiguration configuration) 
            : base(next)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public override async Task Invoke(IOwinContext context)
        {
            var identifier = _configuration.ClientIdentifierFunc(context);
            var store = _configuration.RequestStore;
            var config = new LeakyBucketContainerConfiguration
            {
                RefreshRate = _configuration.RefreshRate,
                Limit = _configuration.MaxNumberOfRequests
            };
            var container = new LeakyBucketContainer(store, config);

            if (container.RequestsRemaining(identifier) <= 0)
            {
                context.Response.StatusCode = ExtendedHttpStatusCodes.TooManyRequests;
            }

            await Next.Invoke(context);
        }
    }
}