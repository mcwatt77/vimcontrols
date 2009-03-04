using System.Windows;
using System.Windows.Controls;
using ActionDictionary;
using ActionDictionary.Interfaces;
using AppControlInterfaces.NoteViewer;

namespace AppViewer.Controls
{
    //TODO: Everything this does should be able to be handled by some sort of layout class... that uses sub controls
        //But maybe only through a template type object...  it could bind controls to the Controller
    public class NoteViewerControl<TDataProcessor> : IAppControl, IMissing, INoteViewUpdate
        where TDataProcessor : INoteViewData
    {
        private readonly TDataProcessor _processor;
        private UIElement _control;
        private UIElement _updateControl;
        private TextBlock _textControl;

        public NoteViewerControl(TDataProcessor processor)
        {
            _processor = processor;
            _processor.Updater = this;
        }

        public UIElement GetControl()
        {
            if (_control == null)
            {
                var grid = new Grid {ShowGridLines = true};
                grid.ColumnDefinitions.Add(new ColumnDefinition{Width = new GridLength(0.3, GridUnitType.Star)});
                grid.ColumnDefinitions.Add(new ColumnDefinition{Width = new GridLength(0.7, GridUnitType.Star)});

                _updateControl = new ListViewCanvas(_processor.GetData, () => _processor.HilightIndex, _processor.RowCount, _processor.ColCount);
                grid.Children.Add(_updateControl);
                Grid.SetColumn(_updateControl, 0);

                _textControl = new TextBlock {Text = _processor.GetData(_processor.HilightIndex, 1)};
                grid.Children.Add(_textControl);
                Grid.SetColumn(_textControl, 1);

                _control = grid;
            }
            return _control;
        }

        public void ProcessMissingCmd(Message msg)
        {
            msg.Invoke(_processor);
        }

        public void Update(int row, int col)
        {
            _updateControl.InvalidateVisual();
        }

        public void Update(int row)
        {
            _updateControl.InvalidateVisual();
        }

        public void UpdateTextBody(string body)
        {
            _textControl.Text = body;
        }
    }
}