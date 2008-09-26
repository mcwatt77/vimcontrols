using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;
using VIMControls.Input;
using VIMControls.Interfaces;
using VIMControls.Interfaces.Framework;
using ICommand=VIMControls.Interfaces.ICommand;

namespace VIMControls.Input
{
    public class KeyModeMap
    {
        private readonly Dictionary<KeyInputMode, Dictionary<Key, IEnumerable<ICommand>>> _map = new Dictionary<KeyInputMode, Dictionary<Key, IEnumerable<ICommand>>>();

        public IEnumerable<ICommand> this[KeyInputMode mode, Key key]
        {
            get
            {
                if (_map.ContainsKey(mode))
                    if (_map[mode].ContainsKey(key))
                        return _map[mode][key];
                return null;
            }
            set
            {
                Dictionary<Key, IEnumerable<ICommand>> keyLookup;
                if (!_map.ContainsKey(mode))
                {
                    keyLookup = new Dictionary<Key, IEnumerable<ICommand>>();
                    _map[mode] = keyLookup;
                }
                else
                    keyLookup = _map[mode];
                keyLookup[key] = value;
            }
        }
    }

    public class KeyCommandGenerator : IKeyCommandGenerator
    {
        private readonly IFactory<ICommand> _commandFactory;
        private KeyModeMap _map;
        private readonly Dictionary<string, IEnumerable<Key>> _stringToKeysMap = new Dictionary<string, IEnumerable<Key>>();


        public KeyCommandGenerator(IFactory<ICommand> commandFactory)
        {
            _commandFactory = commandFactory;
        }

        public KeyModeMap InitializeKeysToCommands()
        {
            _map = new KeyModeMap();

            var methods = typeof (KeyCommandGenerator)
                .Assembly
                .GetMethodsWithCustomAttribute<KeyMapAttribute>()
                .ToList();

//            methods.Do(method => method.AttributesOfType<KeyMapAttribute>().Do(attr => attr.AddToMap(map, method)));
            methods
                .Do(method => method.AttributesOfType<KeyMapAttribute>()
                                  .Do(attr => attr.AddToMap((inputMode, key, @params) =>
                                                            AddCommand(inputMode, key, method, @params))));

            _map[KeyInputMode.Normal, Key.I] = new List<ICommand> {BuildCommand<IApplication>(a => a.SetMode(KeyInputMode.TextInsert))};
            _map[KeyInputMode.Normal, Key.OemQuestion] = new List<ICommand> {BuildCommand<IApplication>(a => a.SetMode(KeyInputMode.Search))};

            return _map;
        }

        private void AddCommand(KeyInputMode inputMode, Key key, MethodInfo method, params object[] @params)
        {
            var cmds = _map[inputMode, key];
            var list = cmds == null ? new List<ICommand>() : cmds.ToList();
            list.Add(_commandFactory.Create(method.BuildLambda(@params)));
            _map[inputMode, key] = list;
        }

        private ICommand BuildCommand<T>(Expression<Action<T>> cmd)
        {
            return _commandFactory.Create(cmd);
        }

        public IEnumerable<ICommand> ProcessKey(Key key)
        {
            var cmds = _map[Mode, key];
            if (cmds == null) throw new Exception("Key " + key + " not found for " + Mode);
            return cmds;
        }

        public IEnumerable<ICommand> ProcessKeyString(string keyString)
        {
            var keys = new List<Key>();
            for (var i = 0; i < keyString.Length; i++)
            {
                if (keyString[i] == ' ')
                {
                    keys.Add(Key.Space);
                    continue;
                }
                if (keyString[i] == '<')
                {
                    i++;
                    var lookupString = String.Empty;
                    while (keyString[i] != '>')
                        lookupString += keyString[i++];
                    //lookup string

                    if (lookupString == "cr")
                    {
                        keys.Add(Key.Return);
                        continue;
                    }
                    throw new Exception("Unknown!");
                }
                var map = "/ OemQuestion ! LeftShift,Oem1 : LeftShift,OemSemicolon";
                var pairs = map.Split(' ');
                Enumerable.Range(0, pairs.Length/2)
                    .Do(idx => _stringToKeysMap[pairs[idx*2]] = pairs[idx*2 + 1]
                        .Split(',')
                        .Select(key => (Key)Enum.Parse(typeof(Key), key)));

                if (_stringToKeysMap.ContainsKey(keyString[i].ToString()))
                    keys.AddRange(_stringToKeysMap[keyString[i].ToString()]);
                else
                    keys.Add((Key) Enum.Parse(typeof (Key), keyString[i].ToString().ToUpper()));
            }

            return keys.Select(key => ProcessKey(key)).Flatten();
        }

        public KeyInputMode Mode { get; private set; }
        public void SetMode(KeyInputMode mode)
        {
            Mode = mode;
        }
    }
}