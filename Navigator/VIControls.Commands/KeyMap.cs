using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using VIControls.Commands.Interfaces;

namespace VIControls.Commands
{
    public class KeyMap
    {
        private static readonly List<KeyMap> KeyMaps
            = new List<KeyMap>
                  {
                      new Map<IVerticallyNavigable>(CommandMode.Normal, Key.J, x => x.MoveVertically(1)),
                      new Map<IVerticallyNavigable>(CommandMode.Normal, Key.K, x => x.MoveVertically(-1)),

                      new Map<INavigable>(CommandMode.Normal, Key.Enter, x => x.NavigateToCurrentChild()),

                      new Map<IHasSearchMode>(CommandMode.Normal, Key.OemQuestion, x => x.EnterSearchMode()),

                      new Map<IUIPort>(CommandMode.Normal, Key.Back, x => x.Back()),

                      new Map<ICharacterEdit>(CommandMode.Insert, Key.A, x => x.Output('a')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.B, x => x.Output('b')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.C, x => x.Output('c')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.D, x => x.Output('d')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.E, x => x.Output('e')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.F, x => x.Output('f')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.G, x => x.Output('g')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.H, x => x.Output('h')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.I, x => x.Output('i')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.J, x => x.Output('j')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.K, x => x.Output('k')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.L, x => x.Output('l')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.M, x => x.Output('m')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.N, x => x.Output('n')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.O, x => x.Output('o')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.P, x => x.Output('p')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.Q, x => x.Output('q')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.R, x => x.Output('r')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.S, x => x.Output('s')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.T, x => x.Output('t')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.U, x => x.Output('u')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.V, x => x.Output('v')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.W, x => x.Output('w')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.X, x => x.Output('x')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.Y, x => x.Output('y')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.Z, x => x.Output('z')),
                      new Map<ICharacterEdit>(CommandMode.Insert, Key.Enter, x => x.NewLine()),

                      new Map<ISearchEdit>(CommandMode.Search, Key.A, x => x.Output('a')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.B, x => x.Output('b')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.C, x => x.Output('c')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.D, x => x.Output('d')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.E, x => x.Output('e')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.F, x => x.Output('f')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.G, x => x.Output('g')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.H, x => x.Output('h')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.I, x => x.Output('i')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.J, x => x.Output('j')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.K, x => x.Output('k')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.L, x => x.Output('l')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.M, x => x.Output('m')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.N, x => x.Output('n')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.O, x => x.Output('o')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.P, x => x.Output('p')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.Q, x => x.Output('q')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.R, x => x.Output('r')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.S, x => x.Output('s')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.T, x => x.Output('t')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.U, x => x.Output('u')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.V, x => x.Output('v')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.W, x => x.Output('w')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.X, x => x.Output('x')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.Y, x => x.Output('y')),
                      new Map<ISearchEdit>(CommandMode.Search, Key.Z, x => x.Output('z')),
                  };

        private static readonly Dictionary<CommandMode, Dictionary<Key, Message>> CompiledKeyMap = GetCompiledKeyMap();

        private static Dictionary<CommandMode, Dictionary<Key, Message>> GetCompiledKeyMap()
        {
            return KeyMaps
                .GroupBy(map => map.Mode)
                .ToDictionary(g => g.Key,
                              g => g.ToDictionary(map => map.Key, map => map.Message));
        }

        public static Message GetMessage(CommandMode commandMode, Key key)
        {
            if (!CompiledKeyMap.ContainsKey(commandMode)) return null;
            var keyMap = CompiledKeyMap[commandMode];
            if (!keyMap.ContainsKey(key)) return null;
            return keyMap[key];
        }

        private class Map<T> : KeyMap
        {
            public Map(CommandMode mode, Key key, Expression<Action<T>> expression)
            {
                Mode = mode;
                Key = key;
                Message = new Message(expression);
            }
        }

        private CommandMode Mode { get; set; }
        private Key Key { get; set; }
        private Message Message { get; set; }
    }
}