using VIMControls.Interfaces;

namespace VIMControls.Interfaces
{
    public interface IStackView : IView
    {
        IExpression Pop();
    }
}