using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UITemplateViewer.Element;

namespace UITemplateViewer.WPF
{
    public class EntityList : IEntityList, IContainer, IUIInitialize
    {
        private StackPanel _stackPanel;
        private IEntityRow _selectedRow;

        public string DisplayName { get; set; }
        public IEnumerable<IEntityRow> Rows { get; set; }

        public IEntityRow SelectedRow
        {
            get { return _selectedRow; }
            set
            {
                if (_selectedRow != null) _selectedRow.Selected = false;

                _selectedRow = value;
                _selectedRow.Selected = true;
            }
        }

        public void AddChild(UIElement element)
        {
            _stackPanel.Children.Add(element);
        }

        public void Initialize()
        {
            _stackPanel = new StackPanel();
            Parent.AddChild(_stackPanel);
        }

        public IContainer Parent { get; set; }
    }
}