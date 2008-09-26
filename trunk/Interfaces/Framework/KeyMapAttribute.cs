using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;
using VIMControls.Input;

namespace VIMControls.Interfaces.Framework
{
    public abstract class KeyMapAttribute : Attribute
    {
        public KeyInputMode KeyInputMode { get; protected set; }
        public string CommandString { get; protected set; }
        public object[] Params { get; protected set; }

        private void AddCommand(Key key, KeyModeMap map, MethodInfo method, params object[] @params)
        {
            var cmds = map[KeyInputMode, key];
            List<ICommand> list;
            if (cmds == null) list = new List<ICommand>();
            else list = cmds.ToList();
            list.Add(new Command(BuildLambda(method, @params)));
            map[KeyInputMode, key] = list;
        }

        public void AddToMap(Action<KeyInputMode, Key, object[]> CommandMapper)
        {
            if (CommandString == "/") CommandMapper(KeyInputMode, Key.OemQuestion, Params);
            else if (CommandString == "<cr>") CommandMapper(KeyInputMode, Key.Return, Params);
            else if (CommandString == @"\[a-z]")
            {
                for (var i = 0; i < 26; i++)
                    CommandMapper(KeyInputMode, (Key)Enum.Parse(typeof(Key), ((char)('A' + i)).ToString()), new object[]{(char)('a' + i)});
            }
            else if (CommandString.Length == 1)
                CommandMapper(KeyInputMode, (Key) Enum.Parse(typeof (Key), CommandString.ToUpper()), Params);
        }

        public void AddToMap(KeyModeMap map, MethodInfo method)
        {
            if (CommandString == "/") AddCommand(Key.OemQuestion, map, method, Params);
            else if (CommandString == "<cr>") AddCommand(Key.Return, map, method, Params);
            else if (CommandString == @"\[a-z]")
            {
                for (var i = 0; i < 26; i++)
                    AddCommand((Key)Enum.Parse(typeof(Key), ((char)('A' + i)).ToString()), map, method, (char)('a' + i));
            }
            else if (CommandString.Length == 1)
                AddCommand((Key) Enum.Parse(typeof (Key), CommandString.ToUpper()), map, method, Params);
        }

        private static LambdaExpression BuildLambda(MethodInfo method, params object[] @params)
        {
            var cExprs = @params.Select(o => Expression.Constant(o));
            var parameter = Expression.Parameter(method.ReflectedType, "a");
            var call = cExprs.Count() > 0
                           ? Expression.Call(parameter, method, cExprs.ToArray())
                           : Expression.Call(parameter, method);
            var l = Expression.Lambda(call, parameter);

            return l;
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