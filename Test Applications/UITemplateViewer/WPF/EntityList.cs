using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UITemplateViewer.Element;

namespace UITemplateViewer.WPF
{
    public class EntityList : IEntityList, IContainer, IUIInitialize
    {
        private StackPanel _stackPanel;

        public string DisplayName { get; set; }
        public IEnumerable<IEntityRow> Rows { get; set; }

        public void AddChild(FrameworkElement element)
        {
            _stackPanel.Children.Add(element);
        }

        public FrameworkElement ControlById(string id)
        {
            throw new System.NotImplementedException();
        }

        public void Initialize()
        {
            _stackPanel = new StackPanel();
            if (ID != null) _stackPanel.Name = ID;
            Parent.AddChild(_stackPanel);
        }

        public IContainer Parent { get; set; }
        public string ID { get; set; }
    }
}