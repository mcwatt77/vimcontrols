using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VIControls.Commands;

namespace Navigator.Path.Notes
{
    public class EditableText : ICharacterEdit
    {
        private int _cursorRow;
        private int _cursorColumn;
        private readonly List<string> _lines;

        public EditableText(string text)
        {
            _lines = Regex.Split(text, "\r\n").ToList();
        }

        public void Output(char c)
        {
            while (_cursorRow + 1 > _lines.Count)
                _lines.Add(String.Empty);

            var line = _lines[_cursorRow];
            while (_cursorColumn > line.Length)
                line += " ";

            _lines[_cursorRow] = line.Insert(_cursorColumn, c.ToString());
        }

        public void NewLine()
        {
        }

        public void Backspace()
        {
        }

        public void SetCursor(int row, int column)
        {
            _cursorRow = row;
            _cursorColumn = column;
        }
    }
}