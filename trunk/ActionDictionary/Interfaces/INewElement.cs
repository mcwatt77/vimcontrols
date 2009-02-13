using ActionDictionary.MapAttributes;

namespace ActionDictionary.Interfaces
{
    public interface INewElement
    {
        [KeyMap(InputMode.Normal, "O")]
        void NewAbove();
        [KeyMap(InputMode.Normal, "o")]
        void NewBelow();
    }
}
