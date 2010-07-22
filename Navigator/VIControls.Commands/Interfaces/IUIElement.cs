namespace VIControls.Commands.Interfaces
{
    public interface IUIElement
    {
        void Render(IUIContainer container);
        void SetFocus(bool on);
        void Update();
    }
}