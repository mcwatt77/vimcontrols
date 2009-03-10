namespace AppControlInterfaces.NoteViewer
{
    public interface ITextCursor
    {
        int Row { get; }
        int Column { get; }
        bool InsertMode { get; }
    }
}