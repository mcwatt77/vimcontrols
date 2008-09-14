using AcceptanceTests.Interfaces;

namespace AcceptanceTests.Interfaces
{
    public interface ICreateBrowseElement : IBrowseElement
    {
        IEditor Create { get; }
        IBrowser Browse { get; }
    }
}