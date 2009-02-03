using ActionDictionary.MapAttributes;

namespace ActionDictionary.Interfaces
{
    public interface IModeChange
    {
        [ChangeModeMap("Normal i Insert : Command", "Insert <esc> Normal")]
        void ChangeMode(InputMode mode);
    }
}