namespace NodeMessaging
{
    public interface INode
    {
        string Name { get; }
        IParentNode Parent { get; }
        IParentNode Root { get; }
        void Register<T>(T t);
        T Get<T>() where T : class;
    }
}