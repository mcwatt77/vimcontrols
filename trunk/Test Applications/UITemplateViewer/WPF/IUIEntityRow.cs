using UITemplateViewer.Element;

namespace UITemplateViewer.WPF
{
    public interface IUIEntityRow : IEntityRow, IUIInitialize
    {
        bool Selected { get; set; }
    }
}