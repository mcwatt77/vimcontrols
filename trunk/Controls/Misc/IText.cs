using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VIMControls.Contracts;
using Point=System.Windows.Point;

namespace VIMControls.Controls.Misc
{
    public interface IText : IUIElement
    {
        string Text { get; set; }
        double Height { get; set; }
        double GetWidthAtIndex(int index);
    }

    public class StackPanelEventWrapper : StackPanel, IStackPanel
    {
        public event Action<Size> RenderSizeChanged;
        private readonly EventableList<IUIElement> _list = new EventableList<IUIElement>();

        public StackPanelEventWrapper()
        {
            _list.OnAdd += OnAdd;
        }

        private void OnAdd(IUIElement element)
        {
            var ctrl = (UIElement) element;
            if (ApplyBorders)
            {
                ctrl = new Border
                           {
                               BorderThickness = new Thickness(1),
                               BorderBrush = Brushes.LightGray,
                               Child = ctrl
                           };
            }
            base.Children.Add(ctrl);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            try
            {
                if (RenderSizeChanged != null)
                    RenderSizeChanged(sizeInfo.NewSize);

                base.OnRenderSizeChanged(sizeInfo);
            }
            catch
            {
                Log.Error("OnRenderSizeChanged errored");
            }
        }

        public new IList<IUIElement> Children
        {
            get
            {
                return _list;
            }
        }

        private bool _applyBorders;
        public bool ApplyBorders
        { 
            get
            {
                return _applyBorders;
            }
            set
            {
                _applyBorders = value;
                if (!_applyBorders) return;

                var fElems = base.Children
                    .OfType<FrameworkElement>()
                    .ToList();

                fElems.Do(fe => base.Children.Remove(fe));
                fElems.Select(fe =>
                                  {
                                      var border = new Border
                                                       {
                                                           BorderThickness = new Thickness(1),
                                                           BorderBrush = Brushes.LightGray,
                                                           Child = fe
                                                       };
                                      return border;
                                  })
                    .Do(border => base.Children.Add(border));
            }
        }

        public bool Fill { get; set; }
    }

    public interface IStackPanel : IUIElement, ICanvasChild
    {
        IList<IUIElement> Children { get; }
        bool ApplyBorders { get; set; }
    }

    public interface IStackPanelFactory
    {
        IStackPanel Create();
    }

    public class StackPanelFactory : IStackPanelFactory
    {
        public IStackPanel Create()
        {
            return new StackPanelEventWrapper();
        }
    }

//    public class TextLine : TextBlock, IText
    public class TextLine : Canvas, IText
    {
        private string _text = String.Empty;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                UpdateDrawing();
            }
        }
        private FormattedText _metrics;

        public event Action<Size> RenderSizeChanged;
        public double GetWidthAtIndex(int index)
        {
            var text = _text.Substring(0, Math.Min(index, _text.Length));
            var tf = new Typeface(new FontFamily("Courier New"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var metrics = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, tf, 15, Brushes.Black);

            return metrics.WidthIncludingTrailingWhitespace;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            dc.DrawText(_metrics, new Point(0, 0));
        }

        private void UpdateDrawing()
        {
            var tf = new Typeface(new FontFamily("Courier New"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            _metrics = new FormattedText(Text ?? "", CultureInfo.CurrentCulture, FlowDirection.LeftToRight, tf, 15, Brushes.Black);

            InvalidateVisual();
        }
    }

    public interface ITextFactory
    {
        IText Create();
    }

    public class TextFactory : ITextFactory
    {
        public IText Create()
        {
            return new TextLine();
        }
    }

}