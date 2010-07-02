namespace Navigator.UI
{
    public interface IUIContainer
    {
        TInterface GetInterface<TInterface>() where TInterface : IUIContainer;
    }
}