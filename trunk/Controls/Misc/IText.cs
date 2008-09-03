using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VIMControls.Contracts;

namespace VIMControls.Controls.Misc
{
    public interface IText : IUIElement
    {
        string Text { get; set; }
        double Height { get; set; }
    }

    public class StackPanelEventWrapper : StackPanel, IStackPanel
    {
        public event Action<SizeChangedInfo> RenderSizeChanged;
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
                    RenderSizeChanged(sizeInfo);

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

    public class TextLine : TextBlock, IText
    {
        public event Action<SizeChangedInfo> RenderSizeChanged;
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