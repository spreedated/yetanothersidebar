using NUnit.Framework;

namespace UnitTests.ConversionToolsTests
{
    [TestFixture]
    public class Base64Tests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        [TestCase("Hello World#123/*-[..:t00:]sds", "SGVsbG8gV29ybGQjMTIzLyotWy4uOnQwMDpdc2Rz")]
        [TestCase("", "")]
        [TestCase(null, "")]
        [TestCase(" ", "IA==")]
        [TestCase(" Hello World!", "IEhlbGxvIFdvcmxkIQ==")]
        public void Base64EncodeTests(string test, string result)
        {
            string computedResult = ConversionTools.Base64.Encode(test);
            Assert.That(computedResult, Is.EqualTo(result));
        }

        [Test]
        [TestCase("SGVsbG8gV29ybGQjMTIzLyotWy4uOnQwMDpdc2Rz", "Hello World#123/*-[..:t00:]sds")]
        [TestCase("", "")]
        [TestCase(null, "")]
        [TestCase("IA==", " ")]
        [TestCase("IEhlbGxvIFdvcmxkIQ==", " Hello World!")]
        public void Base64DecodeTests(string test, string result)
        {
            string computedResult = ConversionTools.Base64.Decode(test);
            Assert.That(computedResult, Is.EqualTo(result));
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}
