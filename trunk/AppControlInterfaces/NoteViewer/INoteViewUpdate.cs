using AppControlInterfaces.ListView;

namespace AppControlInterfaces.NoteViewer
{
    public interface INoteViewUpdate : ITextViewUpdate, IListViewUpdate
    {
    }

    public interface ITextViewUpdate
    {
        void UpdateTextRows();
        void UpdateCursor();
    }
}