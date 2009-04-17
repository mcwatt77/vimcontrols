namespace PathContainer.Node
{
    public interface IDataNode
    {
        object Data { get; }
        T Cast<T>();
    }
}