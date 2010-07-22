using VIControls.Commands.Interfaces;

namespace Navigator.UI
{
    public interface IUIElementFactory
    {
        IUIElement GetUIElement(object modelElement);
    }
}