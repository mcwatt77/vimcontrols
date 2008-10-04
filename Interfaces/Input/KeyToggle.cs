using System;

namespace VIMControls.Interfaces.Input
{
    [Flags]
    public enum KeyToggle
    {
        None = 0,
        Shift = 1,
        Ctrl = 2,
        Alt = 4,
        Win = 8
    }
}