using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VIMControls.Contracts;
using Point=System.Windows.Point;

namespace VIMControls.Controls.StackProcessor
{
    public class VIMGraphPanel : IVIMGraphPanel
    {
        private const int _steps = 200;
        private const double _granularity = 0.1;

        private readonly CanvasWrapper _canvas = new CanvasWrapper();
        private Size _size;

        private readonly List<Func<double, double>> _fns = new List<Func<double, double>>();

        public VIMGraphPanel()
        {
            _fns.Add(x => Math.Pow(0.5 * x, 3) - Math.Pow(0.75 * x, 2) + 2);
//            _fns.Add(x => Math.Sin(x));
            _canvas.RenderSizeChanged += Canvas_RenderSizeChanged;
        }

        private Point ConvertToScreenPoint(Point p)
        {
            var xoffset = _size.Width/2;
            var yoffset = _size.Height/2;
            var xscale = _size.Width/10;
            var yscale = _size.Height/-10;
            xscale = yscale = Math.Min(xscale, yscale);

            return new Point(xscale * p.X + xoffset, yscale * p.Y + yoffset);
        }

        private void Canvas_RenderSizeChanged(Size obj)
        {
            _size = obj;

            RefreshFunctionList();
        }

        private void RefreshFunctionList()
        {
            var pen = new Pen(Brushes.Black, 1);

            var drawing = new DrawingVisual();
            var dc = drawing.RenderOpen();
            var points = Enumerable.Range(_steps/-2, _steps)
                .Select(x => _granularity * x)
                .Select(x => new Point(x, _fns.First()(x)))
                .Select(p => ConvertToScreenPoint(p))
                .ToArray();
            Enumerable.Range(0, points.Length - 1)
                .Where(i => points[i + 1].Y > 0 && points[i].Y > 0)
                .Do(i => dc.DrawLine(pen, points[i + 0], points[i + 1]));

            dc.DrawLine(pen, new Point(_size.Width/2, 0), new Point(_size.Width/2, _size.Height));
            dc.DrawLine(pen, new Point(0, _size.Height/2), new Point(_size.Width, _size.Height/2));
            dc.Close();

            _canvas.AddDrawing(drawing.Drawing);
        }

        public IUIElement GetUIElement()
        {
            return _canvas;
        }

        public void Graph(Delegate fn)
        {
            if (fn.Method.GetParameters().Count() != 1) return;

            _fns.Clear();
            _fns.Add((Func<double, double>)fn);

            RefreshFunctionList();
            _canvas.InvalidateVisual();
        }

        public void ResetInput()
        {
        }

        public void MissingModeAction(IVIMAction action)
        {
        }

        public void MissingMapping()
        {
        }
    }

    public class CanvasWrapper : Canvas, IUIElement
    {
        public event Action<Size> RenderSizeChanged;
        private readonly List<Drawing> _drawings = new List<Drawing>();
        private Size _size;

        public void AddDrawing(Drawing drawing)
        {
            _drawings.Clear();
            
            _drawings.Add(drawing);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if (RenderSizeChanged != null) RenderSizeChanged(_size = sizeInfo.NewSize);
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (_drawings.Count == 0 || _size.Height != ActualHeight || _size.Width != ActualWidth)
            {
                RenderSizeChanged(_size = new Size(ActualWidth, ActualHeight));
            }
            if (_drawings.Count == 1)
                dc.DrawDrawing(_drawings[0]);

            base.OnRender(dc);
        }
    }

    public interface IVIMGraphPanel : IVIMControl, IVIMController
    {
        void Graph(Delegate fn);
    }
}