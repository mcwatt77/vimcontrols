namespace NodeMessaging
{
    public interface IFieldAccessor<T>
    {
        T Value { get; set; }
    }
}
