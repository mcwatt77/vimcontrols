namespace NodeMessaging
{
    public interface INode
    {
        string Name { get; }
        IParentNode Parent { get; }
        T Get<T>() where T : class;
        void Register<T>(T t);
    }
}