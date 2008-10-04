using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace VIMControls.Input
{
    public class OldKeyStringParser
    {
        private static readonly Dictionary<string, IEnumerable<Key>> _stringToKeysMap = InitializeKeyMap();

        private static Dictionary<string, IEnumerable<Key>> InitializeKeyMap()
        {
            var ret = new Dictionary<string, IEnumerable<Key>>();
            var map = "/ OemQuestion ! LeftShift,Oem1 : LeftShift,OemSemicolon";
            var pairs = map.Split(' ');

            Enumerable.Range(0, pairs.Length/2)
                .Do(idx => ret[pairs[idx*2]] = pairs[idx*2 + 1]
                    .Split(',')
                    .Select(key => (Key)Enum.Parse(typeof(Key), key)));
            return ret;
        }

        public static IEnumerable<Key> ProcessKeyString(string keyString)
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
                    if (lookupString == "esc")
                    {
                        keys.Add(Key.Escape);
                        continue;
                    }
                    throw new Exception("Unknown special key <" + lookupString + ">!");
                }

                if (_stringToKeysMap.ContainsKey(keyString[i].ToString()))
                    keys.AddRange(_stringToKeysMap[keyString[i].ToString()]);
                else
                    keys.Add((Key) Enum.Parse(typeof (Key), keyString[i].ToString().ToUpper()));
            }
            return keys;
        }
    }
}
