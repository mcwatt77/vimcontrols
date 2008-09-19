using System;

namespace VIMControls.Interfaces.Framework
{
    public class KeyMapAttribute : Attribute
    {
        public KeyInputMode KeyInputMode { get; private set; }
        public string CommandString { get; private set; }
        public object[] Params { get; private set; }

        public KeyMapAttribute(KeyInputMode inputMode, string commandString, params object[] @params)
        {
            KeyInputMode = inputMode;
            CommandString = commandString;
            Params = @params;
        }
    }
}