using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VIControls.Commands.Interfaces;

namespace Navigator.UI
{
    public class StackPanelWrapper : IStackPanel
    {
        private readonly StackPanel _stackPanel;
        private readonly ScrollViewer _scrollViewer;
        private readonly bool _displaySummary;

        public StackPanelWrapper(StackPanel stackPanel, ScrollViewer scrollViewer, bool displaySummary)
        {
            _stackPanel = stackPanel;
            _scrollViewer = scrollViewer;
            _stackPanel.CanVerticallyScroll = true;
            _displaySummary = displaySummary;
        }

        public TInterface GetInterface<TInterface>()
            where TInterface : IUIContainer
        {
            return (TInterface) (object) this;
        }

        public void AddChild(UIElement element)
        {
            var childScroller = element as ScrollViewer;
            if (childScroller != null)
                childScroller.Height = _stackPanel.ActualHeight;
            _stackPanel.Children.Add(element);
        }

        public bool DisplaySummary
        {
            get { return _displaySummary; }
        }

        public void EnsureVisible(UIElement element)
        {
            if (_scrollViewer == null) return;

            var tb = element as TextBlock;
            if (tb != null)
                tb.BringIntoView();

            return;
        }
    }
}