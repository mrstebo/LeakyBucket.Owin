using System;
using FakeItEasy;
using LeakyBucket.Owin.Identifiers;
using LeakyBucket.Owin.Store;
using NUnit.Framework;

namespace LeakyBucket.Owin.Tests.Store
{
    [TestFixture]
    [Parallelizable]
    public class DefaultRequestStoreTests
    {
        [Test]
        public void NumberOfRequestsFor_should_return_number_of_requests_for_identifier()
        {
            var identifier = A.Fake<IClientIdentifier>();
            var store = new DefaultRequestStore(5);

            store.AddRequest(identifier, DateTime.UtcNow);
            store.AddRequest(identifier, DateTime.UtcNow);

            var result = store.NumberOfRequestsFor(identifier);

            Assert.AreEqual(2, result);
        }

        [Test]
        public void AddRequest_should_add_requests_for_identifier()
        {
            var identifier = A.Fake<IClientIdentifier>();
            var store = new DefaultRequestStore(1);

            store.AddRequest(identifier, DateTime.UtcNow);

            Assert.AreEqual(1, store.NumberOfRequestsFor(identifier));
        }

        [Test]
        public void AddRequest_should_not_exceed_max_number_of_requests()
        {
            var identifier = A.Fake<IClientIdentifier>();
            var store = new DefaultRequestStore(2);
            
            store.AddRequest(identifier, DateTime.UtcNow);
            store.AddRequest(identifier, DateTime.UtcNow);
            store.AddRequest(identifier, DateTime.UtcNow);

            Assert.AreEqual(2, store.NumberOfRequestsFor(identifier));
        }

        [Test]
        public void DeleteRequestsOlderThan_should_remove_requests_from_queue_older_than_date()
        {
            var identifier = A.Fake<IClientIdentifier>();
            var store = new DefaultRequestStore(10);
            var oldDateTime = new DateTime(2017, 1, 1, 12, 0, 0);
            var currentDateTime = new DateTime(2017, 1, 1, 13, 0, 0);
            
            store.AddRequest(identifier, oldDateTime);
            store.AddRequest(identifier, oldDateTime);
            store.AddRequest(identifier, oldDateTime);
            store.AddRequest(identifier, currentDateTime);
            store.AddRequest(identifier, currentDateTime);

            store.DeleteRequestsOlderThan(identifier, currentDateTime);

            Assert.AreEqual(2, store.NumberOfRequestsFor(identifier));
        }

        [Test]
        public void DeleteRequestsOlderThan_should_not_fail_when_no_more_requests_for_identifier()
        {
            var identifier = A.Fake<IClientIdentifier>();
            var store = new DefaultRequestStore(2);
            var oldDateTime = new DateTime(2017, 1, 1, 12, 0, 0);
            
            store.AddRequest(identifier, oldDateTime);
            store.AddRequest(identifier, oldDateTime);
            
            Assert.DoesNotThrow(() => store.DeleteRequestsOlderThan(identifier, DateTime.UtcNow));
            Assert.DoesNotThrow(() => store.DeleteRequestsOlderThan(identifier, DateTime.UtcNow));
        }
    }
}