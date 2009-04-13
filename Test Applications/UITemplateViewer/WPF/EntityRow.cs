using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using NodeMessaging;
using UITemplateViewer.Element;
using Utility.Core;

namespace UITemplateViewer.WPF
{
    public class EntityRow : IUIEntityRow
    {
        private Label _label;
        private IEnumerable<IAccessor<string>> _columns;
        private bool _selected;

        public IContainer Parent { get; set; }
        public IParentNode Context { get; set; }

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

        public IEnumerable<IField> Fields { get; set; }

        public IEnumerable<IAccessor<string>> Columns
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
            var values = Fields.Select(field => field.Value).ToList();

            Columns = Fields.Select(field => (IAccessor<string>)new Accessor<string> {Value = field.Value});
            Parent.AddChild(Label);
        }
    }
}