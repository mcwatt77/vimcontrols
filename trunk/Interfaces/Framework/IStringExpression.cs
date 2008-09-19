using VIMControls.Interfaces;

namespace VIMControls.Interfaces
{
    public interface IStringExpression : IExpression
    {
        string Value { get; }
    }

    public interface INumericExpression : IExpression
    {
        double DVal { get; }
    }
}