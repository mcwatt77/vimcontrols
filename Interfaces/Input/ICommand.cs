namespace VIMControls.Interfaces
{
    public interface ICommand
    {
        void Invoke(ICommandable commandable);
    }
}