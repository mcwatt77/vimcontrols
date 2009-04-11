using UITemplateViewer.Element;

namespace UITemplateViewer.WPF
{
    public class Field : IUIInitialize
    {
        public string Value { get; set; }

        public void Initialize()
        {
            throw new System.NotImplementedException();
        }

        public IContainer Parent
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
    }
}
