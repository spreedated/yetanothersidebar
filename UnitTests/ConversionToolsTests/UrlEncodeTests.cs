using NUnit.Framework;

namespace UnitTests.ConversionToolsTests
{
    [TestFixture]
    public class UrlEncodeTests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        [TestCase("Hello World#123/*-[..:t00:]sds", "hello%20world%23123%2f*-%5b..%3at00%3a%5dsds")]
        [TestCase(null, "")]
        [TestCase("", "")]
        [TestCase(" ", "%20")]
        public void UrlEncoderTests(string test, string result)
        {
            string computedResult = ConversionTools.UrlEncode.Encode(test);
            Assert.That(computedResult, Is.EqualTo(result));
        }

        [Test]
        [TestCase("hello%20world%23123%2f*-%5b..%3at00%3a%5dsds", "hello world#123/*-[..:t00:]sds")]
        [TestCase("", "")]
        [TestCase(null, "")]
        public void UrlDecoderTests(string test, string result)
        {
            string computedResult = ConversionTools.UrlEncode.Decode(test);
            Assert.That(computedResult, Is.EqualTo(result));
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}
