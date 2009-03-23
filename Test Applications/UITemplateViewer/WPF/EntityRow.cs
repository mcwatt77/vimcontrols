using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using NodeMessaging;
using UITemplateViewer.Element;

namespace UITemplateViewer.WPF
{
    public class EntityRow : IEntityRow, IUIInitialize
    {
        private Label _label;
        private IEnumerable<IStringProvider> _columns;
        private bool _selected;

        public IContainer Parent { get; set; }

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
                        _label.Content = Columns.First().Text;
                }
                return _label;
            }
        }

        public IEnumerable<IStringProvider> Columns
        {
            get { return _columns; }
            set
            {
                _columns = value;
                Label.Content = _columns.First().Text;
            }
        }

        public void Initialize()
        {
            Parent.AddChild(Label);
        }
    }
}