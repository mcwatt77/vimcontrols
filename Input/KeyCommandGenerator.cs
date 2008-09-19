using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using VIMControls.Interfaces;
using VIMControls.Interfaces.Framework;
using ICommand=VIMControls.Interfaces.ICommand;

namespace VIMControls.Input
{
    public class Command : ICommand
    {
        public Command(Expression expression)
        {}

        public void Invoke(ICommandable commandable)
        {
        }
    }

    public interface ISearchable
    {
        void AddSearchCharacter(char c);
        void FinalizeSearch();
    }

    public interface IMovable
    {
        void MoveRight(int i);
    }

    public interface INavigable
    {
        void Navigate();
    }

    public class KeyCommandGenerator : IKeyCommandGenerator
    {
        private Dictionary<string, IEnumerable<Key>> _stringToKeysMap = new Dictionary<string, IEnumerable<Key>>();
        private static IDictionary<Key, ICommand> _keyToCommand = InitializeKeysToCommands();

        private static IDictionary<Key, ICommand> InitializeKeysToCommands()
        {
            var dict = new Dictionary<Key, ICommand>();
            dict[Key.OemQuestion] = new Command((Expression<Action<IApplication>>)(a => a.SetMode(KeyInputMode.Search)));
            dict[Key.N] = new Command((Expression<Action<ISearchable>>)(a => a.AddSearchCharacter('n')));
            dict[Key.O] = new Command((Expression<Action<ISearchable>>)(a => a.AddSearchCharacter('o')));
            dict[Key.T] = new Command((Expression<Action<ISearchable>>)(a => a.AddSearchCharacter('t')));
            dict[Key.E] = new Command((Expression<Action<ISearchable>>)(a => a.AddSearchCharacter('e')));
            dict[Key.S] = new Command((Expression<Action<ISearchable>>)(a => a.AddSearchCharacter('s')));

            dict[Key.Enter] = new Command((Expression<Action<ISearchable>>)(a => a.FinalizeSearch()));
            dict[Key.OemQuestion] = new Command((Expression<Action<IApplication>>)(a => a.SetMode(KeyInputMode.Normal)));

            dict[Key.OemQuestion] = new Command((Expression<Action<IMovable>>)(a => a.MoveRight(1)));
            dict[Key.OemQuestion] = new Command((Expression<Action<INavigable>>)(a => a.Navigate()));

            return dict;
        }

        public IEnumerable<ICommand> ProcessKey(Key key)
        {
            return new List<ICommand> {_keyToCommand[key]};
        }

        public IEnumerable<ICommand> ProcessKeyString(string keyString)
        {
            var keys = new List<Key>();
            for (var i = 0; i < keyString.Length; i++)
            {
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
                var map = "/ OemQuestion";
                var pairs = map.Split(' ');
                Enumerable.Range(0, pairs.Length/2)
                    .Do(
                    idx =>
                    _stringToKeysMap[pairs[idx*2]] = new List<Key> {(Key) Enum.Parse(typeof (Key), pairs[idx*2 + 1])});

                if (_stringToKeysMap.ContainsKey(keyString[i].ToString()))
                    keys.AddRange(_stringToKeysMap[keyString[i].ToString()]);
                else
                    keys.Add((Key) Enum.Parse(typeof (Key), keyString[i].ToString().ToUpper()));
            }

            return keys.Select(key => ProcessKey(key)).Flatten();
        }
    }
}