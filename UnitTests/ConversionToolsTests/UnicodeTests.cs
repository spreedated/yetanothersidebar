using NUnit.Framework;

namespace UnitTests.ConversionToolsTests
{
    [TestFixture]
    public class UnicodeTests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        [TestCase("Hello World", @"\u0048\u0065\u006c\u006c\u006f\u0020\u0057\u006f\u0072\u006c\u0064")]
        [TestCase(null, "")]
        [TestCase(" ", @"\u0020")]
        public void UnicodeEncodeTests(string test, string result)
        {
            string computedResult = ConversionTools.Unicode.Encode(test);
            Assert.That(computedResult, Is.EqualTo(result));
        }

        [Test]
        [TestCase(@"\u0048\u0065\u006c\u006c\u006f\u0020\u0057\u006f\u0072\u006c\u0064", "Hello World")]
        [TestCase("", "")]
        [TestCase(@"\u0020", " ")]
        [TestCase(@"\0020", " ")]
        [TestCase("0020", " ")]
        [TestCase("dssd2", "")]
        public void UnicodeDecodeTests(string test, string result)
        {
            string computedResult = ConversionTools.Unicode.Decode(test);
            Assert.That(computedResult, Is.EqualTo(result));
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}
