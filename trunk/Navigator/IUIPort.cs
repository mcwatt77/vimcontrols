namespace Navigator
{
    public interface IUIPort
    {
        void Navigate(object element);
        object ActiveModel { get; }
    }
}