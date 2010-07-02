using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Navigator.UI
{
    public class StringSummaryElement : IUIElement
    {
        private readonly TextBlock _summaryBlock;
        private readonly Run _summaryRun;
        private readonly TextBlock _bodyBlock;
        private readonly Run _bodyRun;

        public StringSummaryElement(string summary, string bodyText)
        {
            _summaryRun = new Run(summary);
            _summaryBlock = new TextBlock(_summaryRun);
            _bodyRun = new Run(bodyText);
            _bodyBlock = new TextBlock(_bodyRun);
        }

        public void Render(IUIContainer container)
        {
            var stackPanel = container.GetInterface<IStackPanel>();
            var block = stackPanel.DisplaySummary ? _summaryBlock : _bodyBlock;

            if (block.Parent != null)
                ((StackPanel)block.Parent).Children.Remove(block);
            stackPanel.AddChild(block);
        }

        public void SetFocus(bool on)
        {
            _summaryRun.Background = on ? Brushes.Bisque : Brushes.White;
        }
    }
}