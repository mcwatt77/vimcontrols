using VIMControls.Interfaces;

namespace VIMControls.Interfaces
{
    public interface ITextEditor : IEditor
    {
        string Text { get; set; }
    }
}