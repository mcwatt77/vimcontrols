using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using VIMControls;

namespace KeyStringParser
{
    public class Parser
    {
        private readonly Dictionary<string, IEnumerable<Key>> _stringLookup = new Dictionary<string, IEnumerable<Key>>();
        private readonly Dictionary<char, IEnumerable<Key>> _charLookup = new Dictionary<char, IEnumerable<Key>>();
        private readonly Dictionary<string, IEnumerable<Key>> _variableKeyLookup = new Dictionary<string, IEnumerable<Key>>();

        public IEnumerable<Key> Parse(string input)
        {
            LoadMap();
            //needs to be a normal for loop!
            for (var i = 0; i < input.Length; i++)
            {
                if (input[i] == '<')
                {
                    var startI = i;
                    while (input[i++] != '>'){}
                    foreach (var key in ParseCharacter(input.Substring(startI, i - startI)))
                        yield return key;
                    i--;
                }
                else
                {
                    foreach (var key in ParseCharacter(input[i].ToString()))
                        yield return key;
                }
            }
        }

        private IEnumerable<Key> ParseCharacter(string input)
        {
            if (input[0] != '<') return _charLookup[input[0]];

            var last = input.IndexOf('>');
            var key = input.Substring(1, last - 1);
            if (_stringLookup.ContainsKey(key)) return _stringLookup[key];

            var keyValuePair = _variableKeyLookup
                .Where(keyPair => Regex.IsMatch(key, string.Format(keyPair.Key, ".*")))
                .Single();

            var resultKey = Regex.Replace(input, "<" + string.Format(keyValuePair.Key, "(.*)") + ">", "$1");

            return keyValuePair.Value.Concat(ParseCharacter(resultKey));
        }

        private void LoadMap()
        {
            ReadMapLines()
                .Where(s => s.Length > 0) //Ignore empty lines
                .Where(s => s.Length > 1 ? (s[0] != ';' || s[1] != ';') : false) //Ignore lines that starts with ;;
                .Select(s => s.Split(' '))
                .Do(AddToDictionaries);
        }

        private void AddToDictionaries(string[] mapLine)
        {
            if (mapLine.Length != 2) throw new Exception("Expected a key and value");

            if (mapLine[0].Length == 1)
            {
                _charLookup[mapLine[0][0]] = ParseKeys(mapLine[1]);
            }
            else if (mapLine[0].Length > 1)
            {
                if (mapLine[0][0] != '<' || mapLine[0][mapLine[0].Length - 1] != '>') throw new Exception("If key is longer than one character it must be contained within <>");
                var innerKey = mapLine[0].Substring(1, mapLine[0].Length - 2);
                if (innerKey.Contains("{0}"))
                    _variableKeyLookup[innerKey] = ParseKeys(mapLine[1]);
                else
                    _stringLookup[innerKey] = ParseKeys(mapLine[1]);
            }
        }

        private static IEnumerable<Key> ParseKeys(string textKeys)
        {
            return textKeys
                .Split(',')
                .Select(s => (Key)Enum.Parse(typeof(Key), s));
        }

        private static IEnumerable<string> ReadMapLines()
        {
            var reader = new StreamReader(@"d:\projects\vimcontrols\keystringparser\key.map");
            string curLine;
            while ((curLine = reader.ReadLine()) != null)
                yield return curLine;
        }
    }
}