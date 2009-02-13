using ActionDictionary.MapAttributes;

namespace ActionDictionary.Interfaces
{
    public interface IEnterProcessor
    {
        [KeyMap(InputMode.Normal, "<cr>")]
        void Enter();
    }
}
