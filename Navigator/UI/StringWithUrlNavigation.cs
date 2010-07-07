using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Navigator.UI
{
    public class StringWithUrlNavigation : IUIElement, INavigable
    {
        private readonly string _url;

        public StringWithUrlNavigation(string summary, string url)
        {
            _url = url;
            _summaryRun = new Run(summary);
            _summaryBlock = new TextBlock(_summaryRun) {TextWrapping = TextWrapping.Wrap};

            _browser = new WebBrowser();
        }

        private readonly TextBlock _summaryBlock;
        private readonly Run _summaryRun;
        private readonly WebBrowser _browser;

        public void Render(IUIContainer container)
        {
            var stackPanel = container.GetInterface<IStackPanel>();
            if (!stackPanel.DisplaySummary) return;

            if (_summaryBlock.Parent != null)
                ((StackPanel) _summaryBlock.Parent).Children.Remove(_summaryBlock);
            stackPanel.AddChild(_summaryBlock);
        }

        public void SetFocus(bool on)
        {
            _summaryRun.Background = on ? Brushes.Bisque : Brushes.White;
        }

        public void Navigate()
        {
            var processStartInfo = new ProcessStartInfo {FileName = _url, Verb = "open"};
            Process.Start(processStartInfo);
        }

        public void Back()
        {
        }
    }
}