namespace VIControls.Commands.Interfaces
{
    public interface IUIContainer
    {
        TInterface GetInterface<TInterface>() where TInterface : IUIContainer;
    }
}