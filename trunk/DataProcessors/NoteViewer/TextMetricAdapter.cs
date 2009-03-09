using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ActionDictionary;
using ActionDictionary.Interfaces;
using Utility.Core;

namespace DataProcessors.NoteViewer
{
    public interface IFormattedTextController
    {
        int FirstRow { get; set; }
        IEnumerable<FormattedText> RowsByHeight(double height);
    }

    public class TextMetricAdapter : IFormattedTextController, IMissing, IPaging, IFullNavigation
    {
        public int TopTextRow { get; set; }

        private readonly Func<int, string> _fnGetRow;
        private readonly TextController _controller;

        public TextMetricAdapter(Func<int, string> fnGetRow, TextController controller)
        {
            _fnGetRow = fnGetRow;
            _controller = controller;
        }

        public int FirstRow { get; set; }

        public IEnumerable<FormattedText> RowsByHeight(double height)
        {
            var row = FirstRow;
            var totalHeight = 0.0;
            while (totalHeight < height)
            {
                var text = GetRow(row++);
                totalHeight += text.Height;
                yield return text;
            }
        }

        public FormattedText GetRow(int row)
        {
            var tf = new Typeface(new FontFamily("Courier New"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var textToFormat = _fnGetRow(row);
            textToFormat = textToFormat.Length == 0 ? " " : textToFormat;
            var metrics = new FormattedText(textToFormat, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, tf, 15, Brushes.Black);
            return metrics;
        }

        public void MoveUp()
        {
            _controller.MoveUp();
        }

        public void MoveDown()
        {
            _controller.MoveDown();
        }

        public void Beginning()
        {
            TopTextRow = 0;
            _controller.Beginning();
        }

        public void End()
        {
            _controller.End();
        }

        public void PageUp()
        {
            TopTextRow -= 10;
            if (TopTextRow < 0)
            {
                TopTextRow = 0;
                Enumerable
                    .Range(0, TopTextRow + 10)
                    .Do(i => _controller.MoveUp());
            }
            else Enumerable
                .Range(0, 10)
                .Do(i => _controller.MoveUp());
        }

        public void PageDown()
        {
            TopTextRow += 10;
            Enumerable
                .Range(0, 10)
                .Do(i => _controller.MoveDown());
        }

        public void ProcessMissingCmd(Message msg)
        {
            msg.Invoke(_controller);
        }

        public void MoveRight()
        {
            //TODO: Should not have to wrap this like this
            _controller.MoveRight();
        }

        public void MoveLeft()
        {
            _controller.MoveLeft();
        }
    }
}
