using System;
using LeakyBucket.Owin.Identifiers;
using LeakyBucket.Owin.Internal;
using LeakyBucket.Owin.Store;

namespace LeakyBucket.Owin
{
    internal class LeakyBucketContainer
    {
        private readonly IRequestStore _requestStore;
        private readonly LeakyBucketContainerConfiguration _configuration;
        private readonly ISystemClock _systemClock;
        

        public LeakyBucketContainer(
            IRequestStore requestStore, 
            LeakyBucketContainerConfiguration configuration,
            ISystemClock systemClock = null)
        {
            _requestStore = requestStore ?? throw new ArgumentNullException(nameof(requestStore));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _systemClock = systemClock ?? new SystemClock();
        }

        public int RequestsRemaining(IClientIdentifier identifier)
        {
            UpdateRequestCount(identifier);

            var numberOfRequests = GetNumberOfRequests(identifier);
            var remaining = _configuration.Limit - numberOfRequests;

            return remaining;
        }
        
        private void UpdateRequestCount(IClientIdentifier identifier)
        {
            var expiryDate = _systemClock.UtcNow.Subtract(_configuration.RefreshRate);

            _requestStore.DeleteRequestsOlderThan(identifier, expiryDate);
            _requestStore.AddRequest(identifier, _systemClock.UtcNow);
        }

        private int GetNumberOfRequests(IClientIdentifier identifier)
        {
            return _requestStore.NumberOfRequestsFor(identifier);
        }
    }
}