using AppControlInterfaces.ListView;

namespace AppControlInterfaces.NoteViewer
{
    public interface INoteViewUpdate : IListViewUpdate
    {
        void UpdateTextRows();
        void UpdateCursor();
    }
}