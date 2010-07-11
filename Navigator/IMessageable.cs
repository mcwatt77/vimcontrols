namespace Navigator
{
    public interface IMessageable
    {
        object Execute(Message message);
    }
}