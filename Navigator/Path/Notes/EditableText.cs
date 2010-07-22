using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Utility.Core;
using VIControls.Commands.Interfaces;

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

            if (_lines.Count == 0) return;

            _cursorRow = _lines.Count - 1;
            _cursorColumn = _lines[_cursorRow].Length;
        }

        public void Output(char c)
        {
            var line = CatchupBuffer();
            _lines[_cursorRow] = line.Insert(_cursorColumn, c.ToString());

            _cursorColumn++;
        }

        private string CatchupBuffer()
        {
            while (_cursorRow + 1 > _lines.Count)
                _lines.Add(String.Empty);

            var line = _lines[_cursorRow];
            while (_cursorColumn > line.Length)
                line += " ";
            return line;
        }

        public void NewLine()
        {
            _cursorColumn = 0;
            _cursorRow++;

            CatchupBuffer();
        }

        public void Backspace()
        {
        }

        public string Text
        {
            get
            {
                return _lines.SeparateBy("\r\n");
            }
        }

        public void SetCursor(int row, int column)
        {
            _cursorRow = row;
            _cursorColumn = column;
        }
    }
}