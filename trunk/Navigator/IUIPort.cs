namespace Navigator
{
    public interface IUIPort
    {
        void Back();
        void Navigate(object element);
        object ActiveModel { get; }
    }
}