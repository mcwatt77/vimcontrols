using System.Windows;
using VIControls.Commands.Interfaces;

namespace Navigator.UI
{
    public interface IStackPanel : IUIContainer
    {
        void AddChild(UIElement element);
        bool DisplaySummary { get; }
        void EnsureVisible(UIElement element);
    }
}