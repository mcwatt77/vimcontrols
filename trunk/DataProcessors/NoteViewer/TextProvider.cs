using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataProcessors.NoteViewer
{
    public class TextProvider
    {
        private readonly IList<string> _lines;

        public TextProvider(string text)
        {
            _lines = ToRows(text).ToList();
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

        public void UpdateLine(int row, string data)
        {
            _lines[row] = data;
        }
    }
}