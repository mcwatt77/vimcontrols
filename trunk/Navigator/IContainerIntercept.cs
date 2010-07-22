namespace Navigator
{
    public interface IContainerIntercept
    {
        TResult Get<TResult>(params object[] objects);
    }
}