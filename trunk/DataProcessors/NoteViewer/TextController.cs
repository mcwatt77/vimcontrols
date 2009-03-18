using System.Linq;
using ActionDictionary;
using ActionDictionary.Interfaces;
using AppControlInterfaces.NoteViewer;

namespace DataProcessors.NoteViewer
{
    //TODO: This should really be the one that owns the data
    public class TextController : IFullNavigation, ITextInput, IModeChange
    {
        private readonly TextCursor _cursor;

        private TextProvider _textProvider;
        public TextProvider TextProvider
        {
            get
            {
                return _textProvider;
            }
            set
            {
                _textProvider = value;
                _cursor.Row = 0;
                _cursor.Column = 0;
                Update(true);
            }
        }

        public ITextViewUpdate Updater { get; set; }

        public TextController(TextCursor cursor)
        {
            _cursor = cursor;
        }

        public void MoveUp()
        {
            _cursor.Row--;
            if (_cursor.Row < 0) _cursor.Row = 0;
            Update(false);
        }

        public void MoveDown()
        {
            _cursor.Row++;
            if (_cursor.Row >= _textProvider.Lines.Count())
                _cursor.Row = _textProvider.Lines.Count() - 1;
            else Update(false);
        }

        public void MoveRight()
        {
            _cursor.Column++;
            Update(false);
        }

        public void MoveLeft()
        {
            _cursor.Column--;
            if (_cursor.Column < 0) _cursor.Column = 0;
            Update(false);
        }

        public void Beginning()
        {
            _cursor.Row = 0;
            Update(false);
        }

        public void End()
        {
            _cursor.Row = _textProvider.Lines.Count() - 1;
            Update(false);
        }

        public void PageUp()
        {
            _cursor.Row -= 10;
            MoveUp();
        }

        public void PageDown()
        {
            _cursor.Row += 10;
            MoveDown();
        }

        public void InputCharacter(char c)
        {
            var newLine = CurrentLine.Insert(_cursor.Column, c.ToString());
            _textProvider.UpdateLine(_cursor.Row, newLine);
            _cursor.Column++;
            Update(true);
        }

        private string CurrentLine
        {
            get
            {
                return _textProvider.Lines.ElementAtOrDefault(_cursor.Row) ?? "";
            }
        }

        public void InsertAfterCursor()
        {
            _cursor.InsertMode = true;
            if (CurrentLine.Length > 0) _cursor.Column++;
            Update(false);
        }

        public void InsertBeforeCursor()
        {
            _cursor.InsertMode = true;
            Update(false);
        }

        public void DeleteBeforeCursor()
        {
            if (_cursor.Column <= 0) return;

            var newLine = CurrentLine.Remove(_cursor.Column - 1, 1);
            _textProvider.UpdateLine(_cursor.Row, newLine);
            _cursor.Column--;
            Update(true);
        }

        public void ChangeMode(InputMode mode)
        {
            _cursor.InsertMode = mode == InputMode.Insert;
            Update(false);
        }

        private void Update(bool rows)
        {
            if (_cursor.Column > CurrentLine.Length) _cursor.Column = CurrentLine.Length;

            if (rows) Updater.UpdateTextRows();
            else Updater.UpdateCursor();
        }
    }
}