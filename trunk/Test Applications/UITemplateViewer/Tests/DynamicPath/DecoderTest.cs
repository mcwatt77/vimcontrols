using NUnit.Framework;
using UITemplateViewer.DynamicPath;

namespace UITemplateViewer.Tests.DynamicPath
{
    [TestFixture]
    public class DecoderTest
    {
        [Test]
        public void Test()
        {
            var decode = Decoder.FromPath("[*]{/note}").ToString();
            Assert.Greater(decode.Length, 0);
            decode = Decoder.FromPath("[*]").ToString();
            Assert.Greater(decode.Length, 0);
            decode = Decoder.FromPath("{@descr}").ToString();
            Assert.Greater(decode.Length, 0);
            decode = Decoder.FromPath("{/dyn::rowSelector}").ToString();
            Assert.Greater(decode.Length, 0);
            decode = Decoder.FromPath("[:noteList/@rows]").ToString();
            Assert.Greater(decode.Length, 0);
            decode = Decoder.FromPath("[../@rowSelector]{@body}").ToString();
            Assert.Greater(decode.Length, 0);
        }
    }
}