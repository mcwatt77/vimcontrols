namespace NodeMessaging
{
    public interface INodeImplementor
    {
        string Name { get; }
        IParentNodeImplementor Parent { get; }
    }
}