using AcceptanceTests.Interfaces;

namespace AcceptanceTests.Interfaces
{
    public interface IStackView : IView
    {
        IExpression Pop();
    }
}