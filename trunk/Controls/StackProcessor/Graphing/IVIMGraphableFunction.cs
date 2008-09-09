using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace VIMControls.Controls.StackProcessor.Graphing
{
    public class GraphableList : IVIMGraphableFunction
    {
        private readonly List<double> _data;
        private IPrimitiveFactory _ptFactory;

        public GraphableList(IEnumerable<double> data)
        {
            _data = data.ToList();
        }

        private IGraphPrimitive PrimitiveFromPoint(Point p)
        {
            if (_ptFactory == null) _ptFactory = Services.Locate<IPrimitiveFactory>()();
            var pt = _ptFactory.Create<IPointPrimitive>();
            pt.Value = p;
            return pt;
        }

        public IEnumerable<IGraphPrimitive> FromGraphParameters(GraphParameters parameters)
        {
            var totalX = parameters.MaxX - parameters.MinX;
            var xStep = totalX/_data.Count;
            return _data
                .Select((d, i) => new Point(parameters.MinX + xStep*i, d))
                .Select(p => PrimitiveFromPoint(p));
        }
    }

    public interface IVIMGraphableFunction
    {
        IEnumerable<IGraphPrimitive> FromGraphParameters(GraphParameters parameters);
    }

    public interface IGraphPrimitive
    {}

    public interface IWPFGraphPrimitive
    {
        void Draw(DrawingContext dc, PointMapper mapper);
    }

    public interface IPointPrimitive : IGraphPrimitive
    {
        Point Value { get; set; }
    }

    public class PointPrimitiveWPF : IPointPrimitive, IWPFGraphPrimitive
    {
        public Point Value { get; set;}
        public void Draw(DrawingContext dc, PointMapper mapper)
        {
            var brush = Brushes.Black;
            var pen = new Pen(brush, 1);
            dc.DrawEllipse(brush, pen, mapper.Map(Value), 3, 3);
        }
    }

    public interface IPrimitiveFactory
    {
        TPrimitive Create<TPrimitive>();
    }

    public class WPFPrimitiveFactory : IPrimitiveFactory
    {
        public TPrimitive Create<TPrimitive>()
        {
            return Services.Locate<TPrimitive>()();
        }
    }

    public class GraphParameters
    {
        public double MinY { get; set; }
        public double MaxY { get; set; }
        public double MinX { get; set; }
        public double MaxX { get; set; }
        public double MaxSteps { get; set; }
    }

}