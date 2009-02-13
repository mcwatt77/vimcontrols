namespace AppControlInterfaces.ListView
{
    public interface IListViewUpdate
    {
        void Update(int row, int col);
        void Update(int row);
    }
}