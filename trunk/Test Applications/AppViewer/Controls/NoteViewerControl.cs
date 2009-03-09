﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ActionDictionary;
using ActionDictionary.Interfaces;
using AppControlInterfaces.NoteViewer;
using Utility.Core;

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
//        private TextBlock _textControl;
//        private TextBlock _cursor;
        private Canvas _textCanvas;

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
                grid.SizeChanged += grid_SizeChanged;
                grid.ColumnDefinitions.Add(new ColumnDefinition{Width = new GridLength(0.16, GridUnitType.Star)});
                grid.ColumnDefinitions.Add(new ColumnDefinition{Width = new GridLength(0.84, GridUnitType.Star)});

                _updateControl = new ListViewCanvas(_processor.GetData, () => _processor.HilightIndex, _processor.RowCount, _processor.ColCount);
                grid.Children.Add(_updateControl);
                Grid.SetColumn(_updateControl, 0);

                _textCanvas = new TextEditViewCanvas(i => _processor.GetTextRow(i),
                                                     () => _processor.TextRowCount,
                                                     () => _processor.TopTextRow,
                                                     () => _processor.Cursor.Row,
                                                     () => _processor.Cursor.Column);
/*                _textCanvas = new Canvas();
                _textControl = new TextBlock();
                _textCanvas.Children.Add(_textControl);*/
                grid.Children.Add(_textCanvas);
                Grid.SetColumn(_textCanvas, 1);

                var color = Brushes.DarkCyan.Clone();
                color.Opacity = 0.5;
//                _cursor = new TextBlock {Height = 23, Width = 30, Background = color, Margin = new Thickness(0)};
//                _textCanvas.Children.Add(_cursor);

                UpdateTextRows();

                _control = grid;
            }
            return _control;
        }

        private void grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _processor.Height = ((Grid)GetControl()).ActualHeight;
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

        public void UpdateTextRows()
        {
            _textCanvas.InvalidateVisual();
        }

        public void UpdateCursor()
        {
            _textCanvas.InvalidateVisual();
/*            if (_processor.Cursor != null)
            {
//                _cursor.Margin = new Thickness(_processor.Cursor.Column * 13, _processor.Cursor.Row * 23, 0, 0);
            }*/
        }
    }

    public class TextEditViewCanvas : Canvas
    {
        private readonly Func<int, FormattedText> _fnRows;
        private readonly Func<int> _fnRowCount;
        private readonly Func<int> _fnTopRow;
        private readonly Func<int> _fnCursorRow;
        private readonly Func<int> _fnCursorCol;
        private readonly TextBlock _cursor;

        public TextEditViewCanvas(Func<int, FormattedText> fnRows, Func<int> fnRowCount, Func<int> fnTopRow, Func<int> fnCursorRow, Func<int> fnCursorCol)
        {
            var color = Brushes.DarkCyan.Clone();
            color.Opacity = 0.5;

            var metrics = fnRows(0);
            var geom = metrics.BuildHighlightGeometry(new Point(0, 0), 0, 1);

            _cursor = new TextBlock {Height = geom.Bounds.Height, Width = geom.Bounds.Width, Background = color, Margin = new Thickness(0)};
            Children.Add(_cursor);

            _fnRows = fnRows;
            _fnRowCount = fnRowCount;
            _fnTopRow = fnTopRow;
            _fnCursorRow = fnCursorRow;
            _fnCursorCol = fnCursorCol;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            _cursor.Margin = new Thickness(_cursor.Width * _fnCursorCol(), _cursor.Height * (_fnCursorRow() - _fnTopRow()), 0, 0);
            var top = 0.0;
            Enumerable.Range(_fnTopRow(), _fnRowCount()).Do(row => top = RenderLine(_fnRows(row), dc, top));
        }

        private static double RenderLine(FormattedText text, DrawingContext dc, double top)
        {
            dc.DrawText(text, new Point(0, top));
            return top + text.Height;
        }
    }
}