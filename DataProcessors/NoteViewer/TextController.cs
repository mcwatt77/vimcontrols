using ActionDictionary.Interfaces;
using AppControlInterfaces.NoteViewer;

namespace DataProcessors.NoteViewer
{
    //TODO: This should really be the one that owns the data
    public class TextController : IFullNavigation
    {
        private readonly TextCursor _cursor;
        public INoteViewUpdate Updater { get; set; }

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
            Updater.UpdateCursor();
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
        }

        public void End()
        {
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
    }
}