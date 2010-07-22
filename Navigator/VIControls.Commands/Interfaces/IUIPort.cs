namespace VIControls.Commands.Interfaces
{
    public interface IUIPort
    {
        void Back();
        void Navigate(object element);
        object ActiveModel { get; }
        IUIElement ActiveUIElement { get; }
    }
}