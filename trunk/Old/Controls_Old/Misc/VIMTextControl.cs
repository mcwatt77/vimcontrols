using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using VIMControls.Contracts;
using Point=VIMControls.Contracts.Point;

namespace VIMControls.Controls.Misc
{
    public class VIMTextControl : ITextInputProvider, IVIMTextStorage, IVIMMultiLineTextDisplay
    {
        private readonly ITextFactory _textFactory;
        private readonly IStackPanelFactory _panelFactory;
        protected readonly IStackPanel _panel;

        private const double _lineHeight = 23;
        private int _numberOfLines;
        protected IText[] _textData;

        public VIMTextControl()
        {
            _textFactory = Services.Locate<ITextFactory>()();
            _panelFactory = Services.Locate<IStackPanelFactory>()();

            _panel = _panelFactory.Create();
            _panel.RenderSizeChanged += UpdateRenderMetrics;
            _textData = new TextLine[0];
            _numberOfLines = 1;
            AddNewTextBlocks();
            AddTextBlocksToVisualDisplayFromIndex(0, _panel.Children);
        }

        public bool ApplyBorders
        { 
            get
            {
                return _panel.ApplyBorders;
            }
            set
            {
                _panel.ApplyBorders = value;
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

        public double LineHeight
        {
            get
            {
                if (!ApplyBorders) return _lineHeight;
                return _lineHeight + 2;
            }
        }

        protected virtual void UpdateRenderMetrics(Size sizeInfo)
        {
            CalculateNumberOfLines(sizeInfo);

            if (_numberOfLines == _textData.Length) return;
            if (_numberOfLines > _textData.Length)
                IncreaseSize();
            else if (_numberOfLines < _textData.Length)
                ReduceSize();
        }

        private void CalculateNumberOfLines(Size sizeInfo)
        {
            _numberOfLines = (int)Math.Round(sizeInfo.Height/LineHeight);

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
            AddTextBlocksToVisualDisplayFromIndex(oldLength, _panel.Children);
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

        private void AddTextBlocksToVisualDisplayFromIndex(int oldLength, ICollection<IUIElement> children)
        {
            Enumerable.Range(oldLength, _numberOfLines - oldLength)
                .Do(i =>
                        {
                            var ctrl = _textData[i];
                            ctrl.Height = _lineHeight;
                            children.Add(ctrl);
                        });
        }

        private void AddNewTextBlocks()
        {
            _textData = _textData
                .Concat(Enumerable.Range(0, _numberOfLines - _textData.Length).Select(i => _textFactory.Create()))
                .ToArray();
        }

        public void Output(char c)
        {
            _textData[Pos.Line].Text = _textData[Pos.Line].Text.Insert(Pos.Column, c.ToString());

            VIMMessageService.SendMessage<IVIMTextCursor>(a => a.MoveHorizontally(1));
        }

        public void NewLine()
        {
            VIMMessageService.SendMessage<IVIMTextCursor>(a => a.BeginningOfLine());
            VIMMessageService.SendMessage<IVIMTextCursor>(a => a.MoveVertically(1));
        }

        private VIMTextDataPosition Pos
        {
            get
            {
                VIMTextDataPosition pos = null;
                VIMMessageService.SendMessage<IVIMTextCursor>(a => pos = a.TextPosition);
                return pos ?? new VIMTextDataPosition{Column = _textData[0].Text.Length, Line = 0};
            }
        }

        private void RemoveLine()
        {
            Enumerable.Range(Pos.Line, _textData.Length - Pos.Line - 1)
                .Do(i => _textData[i].Text = _textData[i + 1].Text);
        }

        private void JoinLines(int lowRange, int highRange)
        {
            if (Pos.Line - lowRange > 0)
            {
                _textData[Pos.Line - lowRange].Text += _textData[Pos.Line - lowRange + 1];
                RemoveLine();
                VIMMessageService.SendMessage<IVIMTextCursor>(cursor =>
                                                                  {
                                                                      cursor.MoveVertically(-1);
                                                                      cursor.BeginningOfLine();
                                                                      cursor.MoveHorizontally(Text.Length);
                                                                  });
            }
        }

        public void Backspace()
        {
            if (Pos.Column <= 0)
            {
                JoinLines(-1, 0);
                return;
            }

            Text = Text.Remove(Pos.Column - 1, 1);
            VIMMessageService.SendMessage<IVIMTextCursor>(cursor => cursor.MoveHorizontally(-1));
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
            return _panel;
        }

        public string Text
        {
            get { return _textData[Pos.Line].Text; }
            set { _textData[Pos.Line].Text = value; }
        }

        public Point ConvertPosition(VIMTextDataPosition pos)
        {
            var x = _textData[pos.Line].GetWidthAtIndex(pos.Column);
            return new Point {X = x, Y = _lineHeight*pos.Line};
        }

        public double GetRequiredHeight(int numLines)
        {
            return (_lineHeight + (ApplyBorders ? 3 : 0))*numLines;
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

        public double Height
        {
            set { _panel.Height = value; }
        }

        public double Width
        {
            set { _panel.Width = value; }
        }
    }
}