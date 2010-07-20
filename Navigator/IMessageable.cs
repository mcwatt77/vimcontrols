namespace Navigator
{
    public interface IMessageable
    {
        object Execute(Message message);
        bool CanHandle(Message message);
    }
}