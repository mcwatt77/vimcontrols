using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DataProcessors.NoteViewer
{
    public class TextProvider
    {
        private readonly IEnumerable<string> _lines;

        public TextProvider(string text)
        {
            _lines = ToRows(text);
        }

        public IEnumerable<string> Lines
        {
            get
            {
                return _lines;
            }
        }

        private static IEnumerable<string> ToRows(string input)
        {
            return Regex.Split(input, "\r\n");
        }
    }
}