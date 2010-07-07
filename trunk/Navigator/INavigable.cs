namespace Navigator
{
    public interface INavigable : INavigableObject
    {
        void NavigateToCurrentChild();
        void Back();
    }
}