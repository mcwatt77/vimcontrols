using System.Windows.Input;
using ActionDictionary.MapAttributes;

namespace ActionDictionary.Interfaces
{
    public interface IAliasMap
    {
        [EnumMap(typeof(Key), "Normal <RightShift> LeftShift", "Insert <RightShift> LeftShift", "Command <RightShift> LeftShift")]
        void SetAlias(Key keyAlias);
    }
}