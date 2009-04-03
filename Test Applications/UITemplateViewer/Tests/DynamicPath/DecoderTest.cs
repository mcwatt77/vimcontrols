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
            var decoder = Decoder.FromPath("//bankruptcy[./filers[../@name = '12']]/charlie");
        }
    }
}