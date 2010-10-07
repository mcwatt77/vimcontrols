using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml.Linq;
using Navigator.UI.Attributes;
using VIControls.Commands.Interfaces;

namespace Navigator.UI
{
    public class TableViewier<TTarget> : IUIElement
        where TTarget : IHasColumns<TTarget>
    {
        private IEnumerable<TTarget> _children;

        private readonly TextBlock _block;
        private readonly Run _run;
        private IStackPanel _stackPanel;

        public TableViewier(string summary, IHasRows<TTarget> columns)
        {
            _children = columns.Children;

            _run = new Run(summary);
            _block = new TextBlock(_run) {TextWrapping = TextWrapping.Wrap};
        }

        public void Render(IUIContainer container)
        {
            _stackPanel = container.GetInterface<IStackPanel>();

            if (_stackPanel.DisplaySummary)
            {
                if (_block.Parent != null)
                    ((StackPanel) _block.Parent).Children.Remove(_block);
                _stackPanel.AddChild(_block);
                return;
            }


            var table = new Table();
            var tableRowGroup = new TableRowGroup();

            table.CellSpacing = 0;
            table.Padding = new Thickness(0);
            table.Columns.Add(new TableColumn());
            
            foreach (var row in _children.Select(child => BuildRow(child)))
                tableRowGroup.Rows.Add(row);

            table.RowGroups.Add(tableRowGroup);

            var document = new FlowDocument(table)
                               {
                                   FontSize = 12.0,
                                   PagePadding = new Thickness(0)
                               };

            var reader = new FlowDocumentScrollViewer
                             {
                                 Document = document,
                                 IsToolBarVisible = false,
//                                 Height = 35
                             };
            _stackPanel.AddChild(reader);
        }

        private static TableRow BuildRow(IHasColumns<TTarget> hasColumns)
        {
            var row = new TableRow();

            var cells = hasColumns.Columns
                .Select(column => column.Compile()((TTarget) hasColumns))
                .Select(data => new Run(data))
                .Select(run => new Paragraph(run))
                .Select(paragraph => new TableCell(paragraph));

            foreach (var cell in cells)
                row.Cells.Add(cell);

            return row;
        }

        public void SetFocus(bool on)
        {
            _run.Background = on ? Brushes.Bisque : Brushes.White;

            if (_stackPanel == null || !on) return;
            _stackPanel.EnsureVisible(_block);
        }

        public void Update()
        {
        }
    }

    public class HtmlToWPF : IUIElement
    {
        private readonly XElement _element;
        private readonly TextBlock _dataBlock;
        private IStackPanel _stackPanel;
        private readonly UIElement _uiElement;

        public HtmlToWPF(XElement element)
        {
            _element = element;
            var descendants = _element.Descendants("td").ToArray();
            if (descendants.Length == 1)
                _uiElement = _dataBlock = new TextBlock(new Run(descendants.First().Value));
            else if (_element.Descendants("a").Count() == 1)
                _uiElement = _dataBlock = new TextBlock(new Run(_element.Descendants("a").First().Value));
            else
            {
                var table = new Table();
                var tableRowGroup = new TableRowGroup();
                var row = new TableRow();

                table.CellSpacing = 0;
                table.Padding = new Thickness(0);
                table.Columns.Add(new TableColumn());

                foreach (var cell in descendants.Select(td => new TableCell(new Paragraph(new Run(td.Value)))))
                    row.Cells.Add(cell);

                tableRowGroup.Rows.Add(row);
                table.RowGroups.Add(tableRowGroup);

                var document = new FlowDocument(table)
                                   {
                                       FontSize = 12.0,
                                       PagePadding = new Thickness(0)
                                   };

                var reader = new FlowDocumentScrollViewer
                                 {
                                     Document = document,
                                     IsToolBarVisible = false,
                                     Height = 35
                                 };
                _uiElement = reader;
            }
        }

        public void Render(IUIContainer container)
        {
            _stackPanel = container.GetInterface<IStackPanel>();
            _stackPanel.AddChild(_uiElement);
        }

        public void SetFocus(bool on)
        {
            if (_dataBlock == null) return;

            _dataBlock.Background = on ? Brushes.Bisque : Brushes.White;

            if (_stackPanel == null || !on) return;
            _stackPanel.EnsureVisible(_dataBlock);
        }

        public void Update()
        {
        }
    }
}