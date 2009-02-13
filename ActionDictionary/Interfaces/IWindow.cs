using System;
using ActionDictionary.MapAttributes;

namespace ActionDictionary.Interfaces
{
    public interface IWindow
    {
        [KeyMap(InputMode.Normal, "<f11>")]
        void Maximize();
        void Navigate(Type type);
    }
}
