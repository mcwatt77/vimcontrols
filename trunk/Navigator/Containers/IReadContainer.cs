namespace Navigator.Containers
{
    public interface IReadContainer
    {
        TResult Get<TResult>(params object[] objects);
    }
}