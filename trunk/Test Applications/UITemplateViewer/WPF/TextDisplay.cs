using System.Windows.Controls;
using NodeMessaging;
using UITemplateViewer.Element;

namespace UITemplateViewer.WPF
{
    public class TextDisplay : IAccessor<string>, IUIInitialize
    {
        private TextBlock _text;
        private bool _initialized;

        public string Value
        {
            get { return _initialized ? TextBlock.Text : null; }
            set { if (_initialized) TextBlock.Text = value; }
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
            _initialized = true;
            Parent.AddChild(TextBlock);
        }

        public IContainer Parent { get; set; }
    }
}