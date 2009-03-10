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
                Updater.UpdateTextRows();
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
            Updater.UpdateCursor();
        }

        public void MoveDown()
        {
            _cursor.Row++;
            if (_cursor.Row >= _textProvider.Lines.Count())
                _cursor.Row = _textProvider.Lines.Count() - 1;
            else Updater.UpdateCursor();
        }

        public void MoveRight()
        {
            _cursor.Column++;
            Updater.UpdateCursor();
        }

        public void MoveLeft()
        {
            _cursor.Column--;
            if (_cursor.Column < 0) _cursor.Column = 0;
            Updater.UpdateCursor();
        }

        public void Beginning()
        {
            _cursor.Row = 0;
            Updater.UpdateCursor();
        }

        public void End()
        {
            _cursor.Row = _textProvider.Lines.Count() - 1;
            Updater.UpdateCursor();
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
            _textProvider.UpdateLine(0, c + _textProvider.Lines.First());
            Updater.UpdateTextRows();
        }

        public void InsertAfterCursor()
        {
            _cursor.InsertMode = true;
            Updater.UpdateCursor();
        }

        public void InsertBeforeCursor()
        {
        }

        public void DeleteBeforeCursor()
        {
        }

        public void ChangeMode(InputMode mode)
        {
            _cursor.InsertMode = mode == InputMode.Insert;
            Updater.UpdateCursor();
        }
    }
}