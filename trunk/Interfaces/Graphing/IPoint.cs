using VIMControls.Interfaces;
using VIMControls.Interfaces.Graphing;

namespace VIMControls.Interfaces.Graphing
{
    public interface IPoint
    {
        double x { get; }
        void SetX(IExpression expr);
        double y { get; }
        void SetY(IExpression expr);
        IPlaneSection RefPlane { get; set; }
    }
}