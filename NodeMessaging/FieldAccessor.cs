namespace NodeMessaging
{
    public class FieldAccessor<T> : IFieldAccessor<T>
    {
        public T Value { get; set; }
    }
}