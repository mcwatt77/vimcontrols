using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using ActionDictionary;
using ActionDictionary.Interfaces;
using AppControlInterfaces.NoteViewer;
using Utility.Core;

namespace DataProcessors.NoteViewer
{
    public interface IFormattedTextController
    {
        int FirstRow { get; set; }
        double Height { get; set; }
        IEnumerable<FormattedText> RowsByHeight();
    }

    public class TextMetricAdapter : IFormattedTextController, IMissing, IPaging, IFullNavigation, ITextViewUpdate
    {
        public int TopTextRow { get; set; }
        public double Height { get; set; }
        public ITextViewUpdate Updater { get; set; }

        public TextProvider TextProvider
        {
            get
            {
                return _controller.TextProvider;
            }
            set
            {
                TopTextRow = 0;
                _controller.TextProvider = value;
            }
        }

        private readonly Func<int, string> _fnGetRow;
        private readonly Func<int> _fnRowCount;
        private readonly TextController _controller;

        public TextMetricAdapter(Func<int, string> fnGetRow, Func<int> fnRowCount, TextController controller)
        {
            _fnGetRow = fnGetRow;
            _fnRowCount = fnRowCount;
            _controller = controller;
            _controller.Updater = this;
        }

        public int FirstRow { get; set; }

        public IEnumerable<FormattedText> RowsByHeight()
        {
            var row = FirstRow;
            var totalHeight = 0.0;
            while (totalHeight < Height)
            {
                var text = GetRow(row++);
                totalHeight += text.Height;
                yield return text;
            }
        }

        public void End()
        {
            var row = _fnRowCount() - 1;
            var totalHeight = 0.0;
            while (totalHeight < Height && row >= 0)
            {
                var text = GetRow(row--);
                totalHeight += text.Height;
            }

            if (row <= TopTextRow) return;

            Enumerable.Range(0, row - TopTextRow).Do(i => _controller.MoveDown());
            TopTextRow = row;
        }

        public FormattedText GetRow(int row)
        {
            var tf = new Typeface(new FontFamily("Courier New"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            string textToFormat;
            if (row >= _fnRowCount())
            {
                textToFormat = "~";
            }
            else
            {
                textToFormat = _fnGetRow(row);
                textToFormat = textToFormat.Length == 0 ? " " : textToFormat;
            }
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

        public void UpdateTextRows()
        {
            if (Updater != null) Updater.UpdateTextRows();
        }

        public void UpdateCursor()
        {
            if (Updater != null) Updater.UpdateCursor();
        }
    }
}
