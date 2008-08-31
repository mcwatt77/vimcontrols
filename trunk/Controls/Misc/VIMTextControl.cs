using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VIMControls.Contracts;
using Point=VIMControls.Contracts.Point;

namespace VIMControls.Controls.Misc
{
    public interface ITextInputProvider : IVIMCharacterController, IVIMMotionController, IVIMControl
    {
        string Text { get; set; }
    }

    public class VIMTextControl : StackPanel, ITextInputProvider, IVIMTextStorage, IVIMMultiLineTextDisplay, IVIMController
    {
        private const double _lineHeight = 23;
        private int _numberOfLines;
        protected TextBlock[] _textData;

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
                //look at each child, and replace
                if (_applyBorders)
                {
                    var fElems = Children
                        .OfType<FrameworkElement>()
                        .ToList();

                    fElems.Do(fe => Children.Remove(fe));
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
                        .Do(border => Children.Add(border));
                }
            }
        }

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
            _textData = new TextBlock[0];
            _numberOfLines = 1;
            AddNewTextBlocks();
            AddTextBlocksToVisualDisplayFromIndex(0, Children);
        }

        public double LineHeight
        {
            get
            {
                if (!ApplyBorders) return _lineHeight;
                return _lineHeight + 2;
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
            _numberOfLines = (int)Math.Round(sizeInfo.NewSize.Height/LineHeight);

            if (_numberOfLines > 1000)
            {
                _numberOfLines = 40;

                MessageBox.Show("What the hell is going?  WPF has lost its mind!");
            }
        }

        private void ReduceSize()
        {
            RemoveTextBlocksFromVisualDisplay();
            RemoveTextBlocks();
        }

        private void IncreaseSize()
        {
            var oldLength = _textData.Length;
            AddNewTextBlocks();
            AddTextBlocksToVisualDisplayFromIndex(oldLength, Children);
        }

        private void RemoveTextBlocksFromVisualDisplay()
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
                            FrameworkElement ctrl = _textData[i];
                            ctrl.Height = _lineHeight;
                            if (ApplyBorders)
                            {
                                ctrl = new Border
                                           {
                                               BorderThickness = new Thickness(1),
                                               BorderBrush = Brushes.LightGray,
                                               Child = ctrl
                                           };
                            }
                            children.Add(ctrl);
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
            get { return _textData[0].Text; }
            set { _textData[0].Text = value; }
        }

        public Point ConvertPosition(VIMTextDataPosition pos)
        {
            return new Point {X = 0, Y = _lineHeight*pos.Line};
        }

        public double GetRequiredHeight(int numLines)
        {
            return _lineHeight*numLines;
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