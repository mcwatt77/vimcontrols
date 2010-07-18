using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace Navigator.UI
{
    public class StringSummaryElement : IUIElement
    {
        private readonly TextBlock _summaryBlock;
        private readonly Run _summaryRun;
        private readonly TextBlock _bodyBlock;
        private readonly Run _bodyRun;

        public string Message
        {
            get
            {
                return _summaryRun.Text;
            }
            set
            {
                _summaryRun.Dispatcher.Invoke(DispatcherPriority.Normal,
                                              (ThreadStart) delegate { _summaryRun.Text = value; });
            }
        }

        public StringSummaryElement(string summary, string bodyText)
        {
            _summaryRun = new Run(summary);
            _summaryBlock = new TextBlock(_summaryRun);
            _summaryBlock.TextWrapping = TextWrapping.Wrap;
            _bodyRun = new Run(bodyText);
            _bodyBlock = new TextBlock(_bodyRun);
            _bodyBlock.TextWrapping = TextWrapping.Wrap;
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