namespace VIControls.Commands.Interfaces
{
    public interface IIsIncrementallySearchable : IIsSearchable
    {
        void UpdateSearchText(string searchText);
        void ClearSearch();
    }
}