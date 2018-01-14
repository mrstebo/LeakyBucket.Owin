using System;
using System.Net;
using System.Threading.Tasks;
using FakeItEasy;
using LeakyBucket.Owin.Identifiers;
using LeakyBucket.Owin.Store;
using Microsoft.Owin;
using Microsoft.Owin.Testing;
using NUnit.Framework;
using Owin;

namespace LeakyBucket.Owin.Tests
{
    [TestFixture]
    [Parallelizable]
    public class LeakyBucketMiddlewareTests
    {
        private static TestServer CreateTestServer(LeakyBucketConfiguration configuration)
        {
            return TestServer.Create(app =>
            {
                app.Use(typeof(LeakyBucketMiddleware), configuration);
                app.Run(async context => await context.Response.WriteAsync("Test"));
            });
        }

        [Test]
        public void Should_throw_with_null_configuration()
        {
            Assert.Throws<ArgumentNullException>(() => new LeakyBucketMiddleware(null, null));
        }

        [Test]
        public async Task Should_check_the_number_of_requests_using_the_request_store()
        {
            var store = A.Fake<IRequestStore>();
            var identifier = A.Fake<IClientIdentifier>();
            var configuration = new LeakyBucketConfiguration
            {
                RefreshRate = TimeSpan.FromSeconds(1),
                MaxNumberOfRequests = 10,
                RequestStore = store,
                ClientIdentifierFunc = _ => identifier
            };

            using (var server = CreateTestServer(configuration))
            {
                await server.CreateRequest("/").GetAsync();
            }

            A.CallTo(() => store.NumberOfRequestsFor(identifier))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task Should_use_the_default_client_identifier_if_not_specified_in_config()
        {
            var store = A.Fake<IRequestStore>();
            var configuration = new LeakyBucketConfiguration
            {
                RefreshRate = TimeSpan.FromSeconds(1),
                MaxNumberOfRequests = 10,
                RequestStore = store
            };

            using (var server = CreateTestServer(configuration))
            {
                await server.CreateRequest("/")
                    .AddHeader("User-Agent", "test")
                    .GetAsync();
            }

            A.CallTo(() => store.NumberOfRequestsFor(
                    A<IClientIdentifier>.That.IsInstanceOf(typeof(DefaultClientIdentifier))))
                .MustHaveHappened(Repeated.Exactly.Once);
            A.CallTo(() => store.NumberOfRequestsFor(
                    A<IClientIdentifier>.That.Matches(x => ((DefaultClientIdentifier) x).UserAgentAddress == "test")))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Test]
        public async Task Should_not_return_too_many_requests_if_number_of_requests_is_below_configured_value()
        {
            var store = A.Fake<IRequestStore>();
            var configuration = new LeakyBucketConfiguration
            {
                RefreshRate = TimeSpan.FromSeconds(1),
                MaxNumberOfRequests = 10,
                RequestStore = store
            };

            A.CallTo(() => store.NumberOfRequestsFor(A<IClientIdentifier>.Ignored)).Returns(9);

            using (var server = CreateTestServer(configuration))
            {
                var response = await server.CreateRequest("/").GetAsync();

                Assert.AreNotEqual(ExtendedHttpStatusCodes.TooManyRequests, response.StatusCode);
            }
        }

        [Test]
        public async Task Should_return_too_many_requests_if_number_of_requests_exceeds_configured_value()
        {
            var identifier = A.Fake<IClientIdentifier>();
            var store = A.Fake<IRequestStore>();
            var configuration = new LeakyBucketConfiguration
            {
                RefreshRate = TimeSpan.FromSeconds(1),
                MaxNumberOfRequests = 10,
                RequestStore = store,
                ClientIdentifierFunc = _ => identifier
            };

            A.CallTo(() => store.NumberOfRequestsFor(A<IClientIdentifier>.Ignored)).Returns(11);

            using (var server = CreateTestServer(configuration))
            {
                var response = await server.CreateRequest("/").GetAsync();

                Assert.AreEqual(ExtendedHttpStatusCodes.TooManyRequests, (int) response.StatusCode);
            }
        }

        [Test]
        public async Task Should_return_too_many_requests_when_request_limit_reached()
        {
            var configuration = new LeakyBucketConfiguration
            {
                RefreshRate = TimeSpan.FromSeconds(30),
                MaxNumberOfRequests = 4
            };
            var failedRequests = 0;

            using (var server = CreateTestServer(configuration))
            {
                for (var i = 0; i < 10; i++)
                {
                    var response = await server.CreateRequest("/")
                        .AddHeader("User-Agent", "test")
                        .GetAsync();

                    if ((int) response.StatusCode == ExtendedHttpStatusCodes.TooManyRequests)
                        failedRequests++;
                }
            }

            Assert.AreEqual(7, failedRequests);
        }
    }
}