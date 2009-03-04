using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ActionDictionary;
using ActionDictionary.Interfaces;
using AppControlInterfaces.ListView;
using Utility.Core;

namespace AppViewer.Controls
{
    public class GridControl<TDataProcessor> : IAppControl, IMissing, IListViewUpdate
        where TDataProcessor : IListViewData
    {
        private readonly TDataProcessor _processor;
        private UIElement _control;

        public GridControl(TDataProcessor processor)
        {
            _processor = processor;
            _processor.Updater = this;
        }

        public UIElement GetControl()
        {
            if (_control == null) _control = new ListViewCanvas(_processor.GetData, () => _processor.HilightIndex, _processor.RowCount, _processor.ColCount);
            return _control;
        }

        public void ProcessMissingCmd(Message msg)
        {
            msg.Invoke(_processor);
        }

        public void Update(int row, int col)
        {
//            _processor.GetData(row, col);
            GetControl().InvalidateVisual();
        }

        public void Update(int row)
        {
//            Enumerable.Range(0, _processor.ColCount).Do(i => _processor.GetData(row, i));
            GetControl().InvalidateVisual();
        }
    }

    public class ListViewCanvas : Canvas
    {
        private readonly Func<int, int, string> _fnData;
        private readonly int _rows;
        private readonly int _cols;
        private readonly Func<int> _fnHilight;

        public ListViewCanvas(Func<int, int, string> fnData, Func<int> fnHilight, int rows, int cols)
        {
            _fnData = fnData;
            _fnHilight = fnHilight;
            _rows = rows;
            _cols = cols;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var top = 0.0;
            Enumerable.Range(0, _rows).Do(row => top = RenderRow(dc, top, row));
        }

        private double RenderRow(DrawingContext dc, double top, int row)
        {
            var tf = new Typeface(new FontFamily("Courier New"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var colWidth = ActualWidth/_cols;
            Enumerable.Range(0, _cols).Do(col =>
                                              {
                                                  top += RenderLine(_fnData(row, col), dc, tf, top, colWidth, _fnHilight() == row);
/*                                                  var metrics = new FormattedText(_fnData(row, col), CultureInfo.CurrentCulture,
                                                                                  FlowDirection.LeftToRight, tf, 15, Brushes.Black)
                                                                    {MaxTextWidth = colWidth};
                                                  if (col == 0) top += metrics.Height;
                                                  dc.DrawText(metrics, new Point(col * colWidth, top));*/
                                              });
            return top;
        }

        private static double RenderLine(string text, DrawingContext dc, Typeface tf, double top, double maxWidth, bool hilight)
        {
            var metrics = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, tf, 15, hilight ? Brushes.Blue : Brushes.Black){MaxTextWidth = maxWidth};
            dc.DrawText(metrics, new Point(0, top));
            return top + metrics.Height;
        }
    }
}