namespace AppControlInterfaces.ListView
{
    public interface IListViewData
    {
        string GetData(int row, int col);
        int RowCount { get; }
        int ColCount { get; }
        int HilightIndex { get; }

        IListViewUpdate Updater { set; }
    }
}