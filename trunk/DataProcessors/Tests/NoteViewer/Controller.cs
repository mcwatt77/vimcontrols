using System.Globalization;
using System.Windows;
using System.Windows.Media;
using NUnit.Framework;

namespace DataProcessors.Tests.NoteViewer
{
    [TestFixture]
    public class ControllerTest
    {
        [Test]
        public void Test()
        {
            var text = "Test text";
            var tf = new Typeface(new FontFamily("Courier New"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var metrics = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, tf, 15, Brushes.Black);

            if (metrics.WidthIncludingTrailingWhitespace == 0.0)
                Assert.Fail("That's suprising");
        }
    }
}
