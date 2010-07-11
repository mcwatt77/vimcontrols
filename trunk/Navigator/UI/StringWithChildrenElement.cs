using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Navigator.UI
{
    public class StringWithChildrenElement : IUIElement
    {
        private readonly IEnumerable<IUIElement> _children;
        private readonly TextBlock _block;
        private readonly Run _run;

        public StringWithChildrenElement(string name, IEnumerable<IUIElement> children)
        {
            _children = children;
            _run = new Run(name);
            _block = new TextBlock(_run) {TextWrapping = TextWrapping.Wrap};
        }

        public void Render(IUIContainer container)
        {
            var stackPanel = container.GetInterface<IStackPanel>();

            if (!stackPanel.DisplaySummary)
            {
                if (_children == null) return;

                var newStackPanel = new StackPanel();
                var newStackPanelWrapper = new StackPanelWrapper(newStackPanel, true);

                foreach (var child in _children)
                {
                    if (child == null) continue;

                    child.Render(newStackPanelWrapper);
                }

                stackPanel.AddChild(newStackPanel);
            }
            else
            {
                if (_block.Parent != null)
                    ((StackPanel)_block.Parent).Children.Remove(_block);
                stackPanel.AddChild(_block);
            }
        }

        public void SetFocus(bool on)
        {
            _run.Background = on ? Brushes.Bisque : Brushes.White;
        }
    }
}