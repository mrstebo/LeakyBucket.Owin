using System.Threading.Tasks;
using FakeItEasy;
using LeakyBucket.Owin.Identifiers;
using LeakyBucket.Owin.Store;
using Microsoft.Owin.Testing;
using NUnit.Framework;
using Owin;

namespace LeakyBucket.Owin.Tests
{
    [TestFixture]
    [Parallelizable]
    public class LeakyBucketMiddlewareExtensionsTests
    {
        [Test]
        public async Task UseLeakyBucket_should_attach_middleware()
        {
            var store = A.Fake<IRequestStore>();
            var configuration = new LeakyBucketConfiguration
            {
                RequestStore = store
            };

            using (var server = TestServer.Create(app =>
            {
                app.UseLeakyBucket(configuration);
                app.Run(async ctx => await ctx.Response.WriteAsync("Test"));
            }))
            {
                await server.CreateRequest("/").GetAsync();
            }

            A.CallTo(() => store.NumberOfRequestsFor(A<IClientIdentifier>.Ignored))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}