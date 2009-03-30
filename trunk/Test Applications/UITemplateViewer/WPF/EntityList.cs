using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UITemplateViewer.Element;
using Utility.Core;

namespace UITemplateViewer.WPF
{
    public class EntityList : IEntityList<IUIEntityRow>, IContainer, IUIInitialize
    {
        private StackPanel _stackPanel;

        public string DisplayName { get; set; }
        public IEnumerable<IUIEntityRow> Rows { get; set; }

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
            Parent.AddChild(_stackPanel);

            Rows.Do(row => row.Parent = this);
            Rows.Do(row => row.Initialize());
        }

        public IContainer Parent { get; set; }
    }
}