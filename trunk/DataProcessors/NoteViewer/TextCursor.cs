using AppControlInterfaces.NoteViewer;

namespace DataProcessors.NoteViewer
{
    public class TextCursor : ITextCursor
    {
        public int Row { get; set; }
        public int Column { get; set; }
    }
}