namespace VIControls.Commands.Interfaces
{
    public interface IMessageable
    {
        object Execute(Message message);
        bool CanHandle(Message message);
    }
}