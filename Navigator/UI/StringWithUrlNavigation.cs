using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using VIControls.Commands.Interfaces;

namespace Navigator.UI
{
    public class StringWithUrlNavigation : IUIElement, INavigableObject
    {
        private readonly string _url;

        public StringWithUrlNavigation(string summary, string url)
        {
            _url = url;
            _summaryRun = new Run(summary);
            _summaryBlock = new TextBlock(_summaryRun) {TextWrapping = TextWrapping.Wrap};
        }

        private readonly TextBlock _summaryBlock;
        private readonly Run _summaryRun;

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

        public void Update()
        {
        }

        public void Navigate()
        {
            var processStartInfo = new ProcessStartInfo {FileName = _url, Verb = "open"};
            Process.Start(processStartInfo);
        }
    }
}