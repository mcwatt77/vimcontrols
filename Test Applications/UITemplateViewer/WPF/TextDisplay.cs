using System.Windows.Controls;
using NodeMessaging;
using UITemplateViewer.Element;

namespace UITemplateViewer.WPF
{
    public class TextDisplay : IFieldAccessor<string>, IUIInitialize
    {
        private TextBlock _text;

        public string Value
        {
            get { return TextBlock.Text; }
            set { TextBlock.Text = value; }
        }

        private TextBlock TextBlock
        {
            get
            {
                if (_text == null)
                    _text = new TextBlock();
                return _text;
            }
        }

        public void Initialize()
        {
            Parent.AddChild(TextBlock);
        }

        public IContainer Parent { get; set; }
        public string ID { get; set; }
    }
}