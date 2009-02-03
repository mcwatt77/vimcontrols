using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VIMControls.Controls;

namespace VIMControls.Contracts
{
    public class VIMTextCursor : TextBlock, IVIMTextCursor, ICanvasChild
    {
        private readonly IVIMTextStorage _textStorage;
        private readonly VIMTextDataPosition _textDataPosition = new VIMTextDataPosition();

        public VIMTextCursor(IVIMTextStorage textStorage)
        {
            var color = Brushes.DarkCyan.Clone();
            color.Opacity = 0.5;
            Background = color;

            _textStorage = textStorage;

            base.Height = 23;
            base.Width = 13;

            _textDataPosition.Column = 0;
            UpdateMargin();
        }

        private void CheckBoundaries()
        {
            if (_textDataPosition.Column < 0) _textDataPosition.Column = 0;
            if (_textDataPosition.Line < 0) _textDataPosition.Line = 0;
        }

        private void UpdateMargin()
        {
            CheckBoundaries();

            var pos = _textStorage.ConvertPosition(_textDataPosition);
            if (pos == null) return;
            Margin = new Thickness(pos.X, pos.Y, 0, 0);
            base.Height = 23;
            base.Width = 13;
        }

        public new double Height
        {
            set
            {}
        }

        public new double Width
        {
            set
            {}
        }

        public void MoveVertically(int i)
        {
            _textDataPosition.Line += i;
            UpdateMargin();
        }

        public void NextLine()
        {
            throw new System.NotImplementedException();
        }

        public IUIElement GetUIElement()
        {
            return new UIElementWrapper(this);
        }

        public VIMTextDataPosition TextPosition
        {
            get { return _textDataPosition; }
        }

        public Point RenderPosition
        {
            get { throw new System.NotImplementedException(); }
        }

        public void MoveHorizontally(int i)
        {
            _textDataPosition.Column += i;
            UpdateMargin();
        }

        public void EndOfLine()
        {
            throw new System.NotImplementedException();
        }

        public void BeginningOfLine()
        {
            _textDataPosition.Column = 0;
        }

        public bool Fill
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
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
}