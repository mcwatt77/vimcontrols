namespace NodeMessaging
{
    public interface INodeImplementor
    {
        string Name { get; }
        IParentNodeImplementor Parent { get; }
        T Get<T>() where T : class;
        void Register<T>(T t);
    }
}