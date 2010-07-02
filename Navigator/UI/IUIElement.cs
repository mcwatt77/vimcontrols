namespace Navigator.UI
{
    public interface IUIElement
    {
        void Render(IUIContainer container);
        void SetFocus(bool on);
    }
}