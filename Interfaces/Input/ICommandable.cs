namespace VIMControls.Interfaces
{
    public interface ICommandable
    {
        void ProcessMissingCommand(ICommand command);
    }
}