using LeakyBucket.Owin.Identifiers;
using NUnit.Framework;

namespace LeakyBucket.Owin.Tests.Identifiers
{
    [TestFixture]
    [Parallelizable]
    public class DefaultClientIdentifierTests
    {
        [Test]
        public void Equals_should_return_true_if_other_has_same_UserAgent()
        {
            var identifier = new DefaultClientIdentifier("test");
            var other = new DefaultClientIdentifier("test");

            Assert.IsTrue(identifier.Equals(other));
        }

        [Test]
        public void Equals_should_return_false_if_other_has_different_UserAgent()
        {
            var identifier = new DefaultClientIdentifier("test");
            var other = new DefaultClientIdentifier("other");

            Assert.IsFalse(identifier.Equals(other));
        }
    }
}