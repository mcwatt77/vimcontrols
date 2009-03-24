namespace UITemplateViewer.Element
{
    public interface IUIInitialize
    {
        void Initialize();
        IContainer Parent { get; set; }
        string ID { get; set; }
    }
}