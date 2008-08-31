using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VIMControls.Controls;

namespace VIMControls.Contracts
{
    public class VIMListCursor : TextBlock, IVIMListCursor, ICanvasChild
    {
        private readonly IVIMTextStorage _textStorage;
        private readonly VIMTextDataPosition _textDataPosition = new VIMTextDataPosition();

        public VIMListCursor(IVIMTextStorage textStorage)
        {
            var color = Brushes.DarkCyan.Clone();
            color.Opacity = 0.5;
            Background = color;

            _textStorage = textStorage;

            base.Height = 23;
        }

        public void MoveVertically(int i)
        {
            _textDataPosition.Line += i;
            if (_textDataPosition.Line < 0)
                _textDataPosition.Line = 0;

            var pos = _textStorage.ConvertPosition(_textDataPosition);
            Margin = new Thickness(pos.X, pos.Y, 0, 0);

            base.Height = 23;
        }

        public void MoveHorizontally(int i)
        {
        }

        public void EndOfLine()
        {
        }

        public void BeginningOfLine()
        {
        }

        public void NextLine()
        {
            Action<IListController> fn = b => b.Select(_textDataPosition.Line);
            object o = fn;
            var msgSvc = ServiceLocator.FindService<IVIMMessageService>()();
            msgSvc.SendMessage(new VIMAction(o));
            _textDataPosition.Line = 0;
            MoveVertically(0);
        }

        public IUIElement GetUIElement()
        {
            return new UIElementWrapper(this);
        }

        public VIMTextDataPosition TextPosition
        {
            get
            {
                return null;
            }
        }

        public Point RenderPosition
        {
            get
            {
                return null;
            }
        }

        public new double Height
        {
            set
            {}
        }

        public new double Width
        {
            set
            {
                base.Width = value;
            }
        }

        public bool Fill
        {
            get { return true; }
        }
    }

    [DependsOn(typeof(IVIMViewport), typeof(IVIMTextStorage))]
    public interface IVIMListCursor : IVIMMotionController, IVIMControl
    {
        VIMTextDataPosition TextPosition { get; }
        Point RenderPosition { get; }
    }

    public class DependsOn : Attribute
    {
        public DependsOn(params Type[] types)
        {}
    }

    public interface IVIMTextStorage
    {
        Point ConvertPosition(VIMTextDataPosition pos);
    }

    public interface IVIMViewport
    {
        void SetDataLine(int line);
    }

    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
    }

    public class VIMTextDataPosition
    {
        public int Column { get; set; }
        public int Line { get; set; }
    }
}