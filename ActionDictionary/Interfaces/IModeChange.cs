using ActionDictionary.MapAttributes;

namespace ActionDictionary.Interfaces
{
    public interface IModeChange
    {
        [ChangeModeMap("Normal i Insert a Insert : Command", "Insert <esc> Normal", "Command <esc> Normal")]
        void ChangeMode(InputMode mode);
    }
}