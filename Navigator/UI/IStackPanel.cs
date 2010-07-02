using System.Windows;

namespace Navigator.UI
{
    public interface IStackPanel : IUIContainer
    {
        void AddChild(UIElement element);
        bool DisplaySummary { get; }
    }
}