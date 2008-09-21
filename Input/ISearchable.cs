using VIMControls.Interfaces.Framework;

namespace VIMControls.Input
{
    public interface ISearchable
    {
        [KeyMapSearch(@"\[a-z]", "$1")]
        void AddSearchCharacter(char c);
        [KeyMapSearch("<cr>")]
        void FinalizeSearch();
    }
}