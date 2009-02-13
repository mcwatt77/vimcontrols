namespace DataProcessors
{
    public class Text : IText
    {
        public void InputCharacter(char c)
        {
            throw new System.NotImplementedException();
        }

        public void InsertAfterCursor()
        {
            throw new System.NotImplementedException();
        }

        public void InsertBeforeCursor()
        {
            throw new System.NotImplementedException();
        }

        public void DeleteBeforeCursor()
        {
            throw new System.NotImplementedException();
        }

        public string GetText()
        {
            throw new System.NotImplementedException();
        }

        public void InsertText(int index)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveText(int index, int count)
        {
            throw new System.NotImplementedException();
        }

        public string GetLinesInBox(int top, int left, int height, int width)
        {
            return null;
            /*var text = _text.Substring(0, Math.Min(index, _text.Length));
            var tf = new Typeface(new FontFamily("Courier New"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var metrics = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, tf, 15, Brushes.Black);

            return metrics.WidthIncludingTrailingWhitespace;*/
        }
    }
}