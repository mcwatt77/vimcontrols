using System.Windows;

namespace UITemplateViewer.Element
{
    public interface IContainer
    {
        void AddChild(FrameworkElement element);
        FrameworkElement ControlById(string id);
    }
}