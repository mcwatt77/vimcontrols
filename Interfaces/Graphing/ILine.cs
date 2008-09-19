using VIMControls.Interfaces.Graphing;

namespace VIMControls.Interfaces.Graphing
{
    public interface ILine : IGraphable
    {
        IPoint p0 { get; set; }
        IPoint p1 { get; set; }
    }
}