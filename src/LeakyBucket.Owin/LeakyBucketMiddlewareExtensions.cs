using Owin;

namespace LeakyBucket.Owin
{
    public static class LeakyBucketMiddlewareExtensions
    {
        public static IAppBuilder UseLeakyBucket(this IAppBuilder app, LeakyBucketConfiguration configuration)
        {
            return app.Use(typeof(LeakyBucketMiddleware), configuration);
        }
    }
}