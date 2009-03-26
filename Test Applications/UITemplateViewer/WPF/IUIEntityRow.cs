using UITemplateViewer.Element;

namespace UITemplateViewer.WPF
{
    public interface IUIEntityRow : IEntityRow
    {
        bool Selected { get; set; }
    }
}