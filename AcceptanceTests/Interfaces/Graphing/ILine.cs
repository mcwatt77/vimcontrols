namespace AcceptanceTests.Interfaces.Graphing
{
    public interface ILine : IGraphable
    {
        IPoint p0 { get; set; }
        IPoint p1 { get; set; }
    }
}