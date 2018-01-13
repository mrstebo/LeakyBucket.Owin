using System;
using LeakyBucket.Owin.Identifiers;

namespace LeakyBucket.Owin.Store
{
    public interface IRequestStore
    {
        void AddRequest(IClientIdentifier identifier, DateTime systemClockUtcNow);
        void DeleteRequestsOlderThan(IClientIdentifier identifier, DateTime expiryDate);
        int NumberOfRequestsFor(IClientIdentifier identifier);
    }
}