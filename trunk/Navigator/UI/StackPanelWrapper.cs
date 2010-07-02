using System.Windows;
using System.Windows.Controls;

namespace Navigator.UI
{
    public class StackPanelWrapper : IStackPanel
    {
        private readonly StackPanel _stackPanel;
        private readonly bool _displaySummary;

        public StackPanelWrapper(StackPanel stackPanel, bool displaySummary)
        {
            _stackPanel = stackPanel;
            _displaySummary = displaySummary;
        }

        public TInterface GetInterface<TInterface>()
            where TInterface : IUIContainer
        {
            return (TInterface) (object) this;
        }

        public void AddChild(UIElement element)
        {
            _stackPanel.Children.Add(element);
        }

        public bool DisplaySummary
        {
            get { return _displaySummary; }
        }
    }
}