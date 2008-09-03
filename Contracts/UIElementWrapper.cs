using System;
using System.Windows;

namespace VIMControls.Contracts
{
    public class UIElementWrapper : IUIElement
    {
        public UIElement UiElement { get; set; }

        public UIElementWrapper(UIElement uiElement)
        {
            UiElement = uiElement;
        }

        public static UIElement From(IVIMControl control)
        {
            var wrapper = control.GetUIElement() as UIElementWrapper;
            if (wrapper == null) return (UIElement)control.GetUIElement();

            return wrapper.UiElement;
        }

        public event Action<SizeChangedInfo> RenderSizeChanged;
    }

    public interface IVIMControl
    {
        IUIElement GetUIElement();
    }

    public interface IUIElement
    {
        event Action<SizeChangedInfo> RenderSizeChanged;
    }
}