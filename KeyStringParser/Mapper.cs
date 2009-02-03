using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Utility.Core;

namespace KeyStringParser
{
    public class Mapper<TValue>
    {
        private readonly Dictionary<Key, object> _map = new Dictionary<Key, object>();
        //todo: this should really hold either a TValue or another dictionary

        public void MapAny(string mapString, TValue obj)
        {
            //todo: should map capital letters as sequence from a dictionary at key LShift

            var capsLock = false;
            var map = _map;
            var p = new Parser();
            var keys = p.Parse(mapString);
            keys = ReplaceCapsLock(keys);
            keys.Do(key => MapKey(key, _map, ref map, ref capsLock, obj));
        }

        private static IEnumerable<Key> ReplaceCapsLock(IEnumerable<Key> keys)
        {
            var indexes = keys.FindAllIndexes(key => key == Key.CapsLock);

            var list = new List<Key>();
            var prevI = 0;
            foreach (var i in indexes)
            {
                var curKeys = keys.Skip(prevI).Take(i - prevI - 1);
                if (i % 2 == 0)
                    list = list.Concat(curKeys).ToList();
                else
                {
                    var e = curKeys.GetEnumerator();
                    while (e.MoveNext())
                    {
                        list.Add(Key.LeftShift);
                        list.Add(e.Current);
                    }
                }
                prevI = i + 1;
            }

            return list;
        }

        private static Dictionary<Key, object> GetOrNew(IDictionary<Key, object> dict, Key key)
        {
            Dictionary<Key, object> ret;
            if (!dict.ContainsKey(key))
            {
                ret = new Dictionary<Key, object>();
                dict[key] = ret;
            }
            else
                ret = (Dictionary<Key, object>)dict[key];
            return ret;
        }

        private static void MapKey(Key key, Dictionary<Key, object> baseMap, ref Dictionary<Key, object> map, ref bool capsLock, TValue obj)
        {
            var specialKeys = new Dictionary<Key, bool>
                                  {
                                      {Key.LeftShift, true},
                                      {Key.RightShift, true},
                                      {Key.LeftCtrl, true},
                                      {Key.RightCtrl, true},
                                      {Key.LeftAlt, true},
                                      {Key.RightAlt, true}
                                  };
            if (specialKeys.ContainsKey(key))
            {
                map = GetOrNew(baseMap, key);
                return;
            }
            if (key == Key.CapsLock)
            {
                capsLock = !capsLock;
                return;
            }

            if (capsLock)
                map = GetOrNew(baseMap, Key.LeftShift);

            map[key] = obj;
            map = baseMap;
        }

        public void MapSequence(string mapString, TValue obj)
        {
        }
    }

    public class KeyMapAttribute : Attribute
    {
        public KeyMapAttribute(string mapString, object val)
        {}
    }

    public class ActionKeyMapAttribute : KeyMapAttribute
    {
        public ActionKeyMapAttribute(string mapString, Action action) : base(mapString, action) { }
        public ActionKeyMapAttribute(string mapString, Action<char> action) : base(mapString, action) { }
    }
}
