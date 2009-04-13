namespace NodeMessaging
{
    public interface IAccessor<T>
    {
        T Value { get; set; }
    }
}
