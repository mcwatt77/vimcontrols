using System.Collections.Generic;
using System.Windows.Input;
using VIMControls.Interfaces.Input;
using ICommand=VIMControls.Interfaces.ICommand;

namespace VIMControls.Input
{
    public class KeyModeMap
    {
        private readonly Dictionary<KeyToggle, Dictionary<KeyInputMode, Dictionary<Key, IEnumerable<ICommand>>>> _map
            = new Dictionary<KeyToggle, Dictionary<KeyInputMode, Dictionary<Key, IEnumerable<ICommand>>>>();

        public IEnumerable<ICommand> this[KeyInputMode mode, KeyToggle toggle, Key key]
        {
            get
            {
                if (_map.ContainsKey(toggle))
                    if (_map[toggle].ContainsKey(mode))
                        if (_map[toggle][mode].ContainsKey(key))
                            return _map[toggle][mode][key];
                return null;
            }
            set
            {
                var modeLookup = _map.GetOrNew(toggle);
                var keyLookup = modeLookup.GetOrNew(mode);
                keyLookup[key] = value;
            }
        }
    }
}