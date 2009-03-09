using System.Windows.Media;

namespace AppControlInterfaces.NoteViewer
{
    public interface INoteViewData : ILeftNavData, ITextData
    {
        INoteViewUpdate Updater { set; }
        double Height { get; set; }
        double Width { get; set; }
    }

    public interface ILeftNavData
    {
        string GetData(int row, int col);
        int RowCount { get; }
        int ColCount { get; }
        int HilightIndex { get; }
    }

    public interface ITextData
    {
        FormattedText GetTextRow(int row);
        int TextRowCount { get; }
        int TopTextRow { get; }

        ITextCursor Cursor { get; }
    }
}