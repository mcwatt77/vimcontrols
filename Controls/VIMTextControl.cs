using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VIMControls.Controls
{
    public interface ITextInputProvider : IVIMCharacterController, IVIMMotionController, IVIMControl
    {
        string Text { get; set; }
    }

    public interface IVIMControl
    {
        IUIElement GetUIElement();
    }

    public interface IUIElement
    {
    }

    public class UIElementWrapper : IUIElement
    {
        public UIElement UiElement { get; set; }

        public UIElementWrapper(UIElement uiElement)
        {
            UiElement = uiElement;
        }
    }

    public class VIMTextControl : StackPanel, ITextInputProvider
    {
        private const double _lineHeight = 23;
        private int _numberOfLines;
        protected TextBlock[] _textData;

        public string Value
        {
            get
            {
                return _textData[0].Text;
            }
            set
            {
                _textData[0].Text = value;
            }
        }

        public VIMTextControl()
        {
            _textData = new []{new TextBlock()};
            _textData[0].Height = _lineHeight;

            var border = new Border
                             {
                                 BorderThickness = new Thickness(1),
                                 BorderBrush = Brushes.LightGray,
                                 Child = _textData[0]
                             };
            Children.Add(border);
//            Children.Add(_textData[0]);

            _numberOfLines = _textData.Length;
        }

        public double LineHeight
        {
            get
            {
                return _lineHeight;
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            try
            {
                CalculateNumberOfLines(sizeInfo);

                if (_numberOfLines == _textData.Length) return;
                if (_numberOfLines > _textData.Length)
                    IncreaseSize();
                else if (_numberOfLines < _textData.Length)
                    ReduceSize();

                base.OnRenderSizeChanged(sizeInfo);
            }
            catch
            {}
        }

        private void CalculateNumberOfLines(SizeChangedInfo sizeInfo)
        {
            _numberOfLines = (int)Math.Round(sizeInfo.NewSize.Height/_lineHeight);
        }

        private void ReduceSize()
        {
            RemoveTextBlocksFromVisualDisplay(Children);
            RemoveTextBlocks();
        }

        private void IncreaseSize()
        {
            var oldLength = _textData.Length;
            AddNewTextBlocks();
            AddTextBlocksToVisualDisplayFromIndex(oldLength, Children);
        }

        private void RemoveTextBlocksFromVisualDisplay(UIElementCollection children)
        {
            _textData
                .Skip(_numberOfLines)
                .Do(text => text.Text = "");
        }

        private void RemoveTextBlocks()
        {
            _textData = _textData
                .Take(_numberOfLines)
                .ToArray();
        }

        private void AddTextBlocksToVisualDisplayFromIndex(int oldLength, UIElementCollection children)
        {
            Enumerable.Range(oldLength, _numberOfLines - oldLength)
                .Do(i =>
                        {
                            _textData[i].Height = _lineHeight;
                            children.Add(_textData[i]);
                        });
        }

        private void AddNewTextBlocks()
        {
            _textData = _textData
                .Concat(Enumerable.Range(0, _numberOfLines - _textData.Length).Select(i => new TextBlock()))
                .ToArray();
        }

        public void Output(char c)
        {
            _textData[0].Text += c;
        }

        public void NewLine()
        {
        }

        public void Backspace()
        {
            if (_textData[0].Text.Length > 0)
                _textData[0].Text = _textData[0].Text.Remove(_textData[0].Text.Length - 1);
        }

        public void MoveVertically(int i)
        {
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
        }

        public IUIElement GetUIElement()
        {
            return new UIElementWrapper(this);
        }

        public string Text
        {
            get { return ""; }
            set { }
        }
    }
}