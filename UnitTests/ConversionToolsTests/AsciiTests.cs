using NUnit.Framework;

namespace UnitTests.ConversionToolsTests
{
    [TestFixture]
    public class AsciiTests
    {
        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        [TestCase("Hello World", "&#72;&#101;&#108;&#108;&#111;&#32;&#87;&#111;&#114;&#108;&#100;")]
        [TestCase(" ", "&#32;")]
        [TestCase(null, "")]
        [TestCase(null, "", false)]
        [TestCase(" ", "32", false)]
        [TestCase("Hello World", "72 101 108 108 111 32 87 111 114 108 100", false)]
        public void AsciiEncodeTests(string test, string result, bool useUrlEncoding = true)
        {
            string computedResult = ConversionTools.Ascii.Encode(test, useUrlEncoding);
            Assert.That(computedResult, Is.EqualTo(result));
        }

        [Test]
        [TestCase("&#72;&#101;&#108;&#108;&#111;&#32;&#87;&#111;&#114;&#108;&#100;", "Hello World")]
        [TestCase("&#32;", " ")]
        [TestCase(null, "")]
        [TestCase("32", " ")]
        [TestCase("s32", "")]
        [TestCase("s32 101", "e")]
        [TestCase("s32 101;&#108", "el")]
        [TestCase("s32 101;&#108;s48", "el")]
        public void AsciiDecodeTests(string test, string result)
        {
            string computedResult = ConversionTools.Ascii.Decode(test);
            Assert.That(computedResult, Is.EqualTo(result));
        }

        [TearDown]
        public void TearDown()
        {

        }
    }
}
