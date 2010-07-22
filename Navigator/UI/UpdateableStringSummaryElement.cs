using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using VIControls.Commands.Interfaces;

namespace Navigator.UI
{
    public class UpdateableStringSummaryElement : IUIElement, IUIElementOnNavigate
    {
        private readonly IHasInsertMode _insertMode;
        private readonly Func<string> _fnBodyText;
        private readonly TextBlock _summaryBlock;
        private readonly Run _summaryRun;
        private readonly TextBlock _bodyBlock;
        private readonly Run _bodyRun;
        private IStackPanel _stackPanel;

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

        public UpdateableStringSummaryElement(IHasInsertMode insertMode, Func<string> fnSummary, Func<string> fnBodyText)
        {
            _insertMode = insertMode;
            _fnBodyText = fnBodyText;
            _summaryRun = new Run(fnSummary());
            _summaryBlock = new TextBlock(_summaryRun);
            _summaryBlock.TextWrapping = TextWrapping.Wrap;
            _bodyRun = new Run(fnBodyText());
            _bodyBlock = new TextBlock(_bodyRun);
            _bodyBlock.TextWrapping = TextWrapping.Wrap;
        }

        public void Render(IUIContainer container)
        {
            _stackPanel = container.GetInterface<IStackPanel>();
            var block = _stackPanel.DisplaySummary ? _summaryBlock : _bodyBlock;

            if (block.Parent != null)
                ((StackPanel)block.Parent).Children.Remove(block);
            _stackPanel.AddChild(block);
        }

        public void SetFocus(bool on)
        {
            _summaryRun.Background = on ? Brushes.Bisque : Brushes.White;

            if (_stackPanel == null || !on) return;
            _stackPanel.EnsureVisible(_summaryBlock);
        }

        public void Update()
        {
            _bodyRun.Text = _fnBodyText();
        }

        public void OnNavigate()
        {
            _insertMode.EnterInsertMode();
        }
    }
}