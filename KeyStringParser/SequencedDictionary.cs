using System;
using System.Collections.Generic;
using System.Linq;
using Utility.Core;

namespace KeyStringParser
{
    public class SequencedDictionary<TKey, TValue>
    {
        private class DictionaryValue
        {
            public IEnumerable<TValue> Value { get; set; }
            public Dictionary<TKey, DictionaryValue> Dict { get; set; }
            public bool UseValueForNoMatch { get; set; }
        }

        public class PopInfo
        {
            public IEnumerable<TKey> Keys { get; internal set; }
            public TValue Value { get; internal set; }
        }

        private readonly Dictionary<TKey, DictionaryValue> _backingStore = new Dictionary<TKey, DictionaryValue>();
        private readonly List<PopInfo> _outputQueue = new List<PopInfo>();
        private readonly List<KeyValuePair<TKey, DictionaryValue>> _queuedPairs = new List<KeyValuePair<TKey, DictionaryValue>>();

        public void Add(IEnumerable<TKey> key, TValue value)
        {
            Add(key, new[] {value});
        }

        public void Add(IEnumerable<TKey> key, IEnumerable<TValue> value)
        {
            Set(key, value, false);
        }

        public void Set(IEnumerable<TKey> key, TValue value)
        {
            Set(key, new[] {value});
        }

        public void Set(IEnumerable<TKey> key, IEnumerable<TValue> value)
        {
            Set(key, value, true);
        }

        private void Set(IEnumerable<TKey> key, IEnumerable<TValue> value, bool replace)
        {
            var dict = _backingStore;
            var e = key.GetEnumerator();

            if (!e.MoveNext()) throw new Exception("Empty key specified");
            var prev = e.Current;

            while (e.MoveNext())
            {
                if (!dict.ContainsKey(prev))
                {
                    var newDict = new DictionaryValue{Dict = new Dictionary<TKey, DictionaryValue>()};
                    dict[prev] = newDict;
                    dict = newDict.Dict;
                }
                else
                    dict = dict[prev].Dict;

                prev = e.Current;
            }

            DictionaryValue dVal;
            if (!dict.ContainsKey(prev))
            {
                dVal = new DictionaryValue();
                dict[prev] = dVal;
            }
            else
                dVal = dict[prev];

            dVal.Value = (replace || dVal.Value == null) ? value : dVal.Value.Concat(value);
            dVal.UseValueForNoMatch = true;
        }

        public int Count()
        {
            return _outputQueue.Count;
        }

        public PopInfo Pop()
        {
            var popInfo = _outputQueue[0];
            _outputQueue.RemoveAt(0);
            return popInfo;
        }

        public void Flush()
        {
            _queuedPairs.Clear();
        }

        //TODO: This seems complicated enough that I'm really thinking this whole method should be a separate class...
        public void Push(TKey key)
        {
            if (_queuedPairs.Count == 0)
            {
                if (!_backingStore.ContainsKey(key))
                {
                    _outputQueue.Add(new PopInfo {Keys = new[] {key}});
                }
                else
                {
                    var val = _backingStore[key];
                    if ((val.Dict != null ? (val.Dict.Count == 0) : true) && val.UseValueForNoMatch)
                    {
                        _queuedPairs.Add(new KeyValuePair<TKey, DictionaryValue>(key, null));
                        val.Value.Do(v => _outputQueue.Add(new PopInfo { Keys = _queuedPairs.Select(keyPair => keyPair.Key).ToList(), Value = v }));
                        _queuedPairs.Clear();
                    }
                    else
                        _queuedPairs.Add(new KeyValuePair<TKey, DictionaryValue>(key, val));
                }
            }
            else
            {
                var currentDict = _queuedPairs.Last().Value;
                if (currentDict.Dict.ContainsKey(key))
                {
                    var val = currentDict.Dict[key];
                    if ((val.Dict != null ? (val.Dict.Count == 0) : true) && val.UseValueForNoMatch)
                    {
                        _queuedPairs.Add(new KeyValuePair<TKey, DictionaryValue>(key, null));
                        val.Value.Do(v => _outputQueue.Add(new PopInfo { Keys = _queuedPairs.Select(keyPair => keyPair.Key).ToList(), Value = v }));
                        _queuedPairs.Clear();
                    }
                    else
                    {
                        _queuedPairs.Add(new KeyValuePair<TKey, DictionaryValue>(key, val));
                    }
                }
                else
                {
                    if (currentDict.UseValueForNoMatch)
                    {
                        currentDict.Value.Do(v => _outputQueue.Add(new PopInfo { Keys = _queuedPairs.Select(keyPair => keyPair.Key).ToList(), Value = v }));
                        _queuedPairs.Clear();
                        Push(key);
                        return;
                    }

                    var index = _queuedPairs.ReverseFindIndex(keyPair => keyPair.Value.UseValueForNoMatch);
                    if (index > -1)
                    {
                        _queuedPairs[index]
                            .Value
                            .Value
                            .Do(v => _outputQueue.Add(new PopInfo{Keys = _queuedPairs.Take(index + 1).Select(keyPair => keyPair.Key).ToList(), Value = v}));

                        _queuedPairs.Add(new KeyValuePair<TKey, DictionaryValue>(key, null));
                        var newList = _queuedPairs.Skip(index + 1).ToList();
                        _queuedPairs.Clear();
                        newList.Do(keyPair => Push(keyPair.Key));
                        return;
                    }

                    _outputQueue.Add(new PopInfo {Keys = new[] {_queuedPairs.First().Key}});

                    _queuedPairs.Add(new KeyValuePair<TKey, DictionaryValue>(key, null));
                    var newList2 = _queuedPairs.Skip(1).ToList();
                    _queuedPairs.Clear();
                    newList2.Do(keyPair => Push(keyPair.Key));
                    return;
                }
            }
        }
    }
}