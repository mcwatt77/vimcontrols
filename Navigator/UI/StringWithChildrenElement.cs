using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Navigator.UI.Attributes;

namespace Navigator.UI
{
    public class StringWithChildrenElement : IUIElement
    {
        private readonly IUIElementFactory _uiElementFactory;
        private readonly IModelChildren _modelElement;
        private readonly TextBlock _block;
        private readonly Run _run;

        public StringWithChildrenElement(string name, IModelChildren modelElement, IUIElementFactory uiElementFactory)
        {
            _modelElement = modelElement;
            _uiElementFactory = uiElementFactory;
            _run = new Run(name);
            _block = new TextBlock(_run);
            _block.TextWrapping = TextWrapping.Wrap;
        }

        public void Render(IUIContainer container)
        {
            var stackPanel = container.GetInterface<IStackPanel>();

            if (!stackPanel.DisplaySummary)
            {
                if (_modelElement == null) return;

                var newStackPanel = new StackPanel();
                var newStackPanelWrapper = new StackPanelWrapper(newStackPanel, true);

                foreach (var child in _modelElement.Children)
                {
                    if (child == null) continue;

                    var uiElement = _uiElementFactory.GetUIElement(child);
                    uiElement.Render(newStackPanelWrapper);
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