namespace NodeMessaging
{
    public interface INode
    {
        string Name { get; }
        IParentNode Parent { get; }
    }
}