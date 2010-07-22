using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using VIControls.Commands.Interfaces;

namespace Navigator.UI
{
    public class StringWithChildrenElement : IUIElement
    {
        private readonly IUIChildren _hasChildren;
        private readonly TextBlock _block;
        private readonly Run _run;
        private IStackPanel _stackPanel;

        public StringWithChildrenElement(string name, IUIChildren hasChildren)
        {
            _hasChildren = hasChildren;
            _run = new Run(name);
            _block = new TextBlock(_run) {TextWrapping = TextWrapping.Wrap};
        }

        public void Render(IUIContainer container)
        {
            _stackPanel = container.GetInterface<IStackPanel>();

            if (!_stackPanel.DisplaySummary)
            {
                if (_hasChildren == null) return;

                var scrollViewer = new ScrollViewer();
                var newStackPanel = new StackPanel();
                scrollViewer.Content = newStackPanel;
                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

                var newStackPanelWrapper = new StackPanelWrapper(newStackPanel, scrollViewer, true);

                foreach (var child in _hasChildren.UIElements)
                {
                    if (child == null) continue;

                    child.Render(newStackPanelWrapper);
                }

                _stackPanel.AddChild(scrollViewer);
            }
            else
            {
                if (_block.Parent != null)
                    ((StackPanel)_block.Parent).Children.Remove(_block);
                _stackPanel.AddChild(_block);
            }
        }

        public void SetFocus(bool on)
        {
            _run.Background = on ? Brushes.Bisque : Brushes.White;

            if (_stackPanel == null || !on) return;
            _stackPanel.EnsureVisible(_block);
        }

        public void Update()
        {
        }
    }
}