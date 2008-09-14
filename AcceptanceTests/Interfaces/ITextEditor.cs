using AcceptanceTests.Interfaces;

namespace AcceptanceTests.Interfaces
{
    public interface ITextEditor : IEditor
    {
        string Text { get; set; }
    }
}