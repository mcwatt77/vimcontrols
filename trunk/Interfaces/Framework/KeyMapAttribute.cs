using System;
using System.Windows.Input;
using VIMControls.Interfaces.Input;

namespace VIMControls.Interfaces.Framework
{
    public abstract class KeyMapAttribute : Attribute
    {
        public KeyInputMode KeyInputMode { get; protected set; }
        public string CommandString { get; protected set; }
        public object[] Params { get; protected set; }

        public void AddToMap(Action<KeyInputMode, KeyToggle, Key, object[]> CommandMapper)
        {
            if (CommandString == "/") CommandMapper(KeyInputMode, KeyToggle.None, Key.OemQuestion, Params);
            else if (CommandString == "<cr>") CommandMapper(KeyInputMode, KeyToggle.None, Key.Return, Params);
            else if (CommandString == @"\[a-z ]")
            {
                for (var i = 0; i < 26; i++)
                    CommandMapper(KeyInputMode, KeyToggle.None, (Key)Enum.Parse(typeof(Key), ((char)('A' + i)).ToString()), new object[]{(char)('a' + i)});
                CommandMapper(KeyInputMode, KeyToggle.None, Key.Space, new object[]{' '});
            }
            else if (CommandString.Length == 1)
                CommandMapper(KeyInputMode, KeyToggle.None, (Key) Enum.Parse(typeof (Key), CommandString.ToUpper()), Params);
        }
    }

    public class KeyMapInsertAttribute : KeyMapAttribute
    {
        public KeyMapInsertAttribute(string commandString, params object[] @params)
        {
            KeyInputMode = KeyInputMode.TextInsert;
            CommandString = commandString;
            Params = @params;
        }
    }

    public class KeyMapNormalAttribute : KeyMapAttribute
    {
        public KeyMapNormalAttribute(string commandString, params object[] @params)
        {
            KeyInputMode = KeyInputMode.Normal;
            CommandString = commandString;
            Params = @params;
        }
    }

    public class KeyMapSearchAttribute : KeyMapAttribute
    {
        public KeyMapSearchAttribute(string commandString, params object[] @params)
        {
            KeyInputMode = KeyInputMode.Search;
            CommandString = commandString;
            Params = @params;
        }
    }
}