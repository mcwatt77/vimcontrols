using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using NodeMessaging;
using UITemplateViewer.Element;

namespace UITemplateViewer.WPF
{
    public class EntityRow : IUIEntityRow, IUIInitialize
    {
        private Label _label;
        private IEnumerable<IFieldAccessor<string>> _columns;
        private bool _selected;

        public IContainer Parent { get; set; }
        public IParentNode Context { get; set; }
        public string ID { get; set; }

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                _label.Background = _selected ? Brushes.SpringGreen : Brushes.White;
            }
        }

        private Label Label
        {
            get
            {
                if (_label == null)
                {
                    _label = new Label();
                    if (Columns != null)
                        _label.Content = Columns.First().Value;
                }
                return _label;
            }
        }

        public IEnumerable<IFieldAccessor<string>> Columns
        {
            get { return _columns; }
            set
            {
                _columns = value;
                Label.Content = _columns.First().Value;
            }
        }

        public void Initialize()
        {
            Parent.AddChild(Label);
        }
    }
}