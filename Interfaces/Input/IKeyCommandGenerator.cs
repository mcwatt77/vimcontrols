using System.Collections.Generic;
using System.Windows.Input;
using VIMControls.Interfaces.Input;

namespace VIMControls.Interfaces.Input
{
    public interface IKeyCommandGenerator
    {
        IEnumerable<ICommand> ProcessKey(Key key);
        IEnumerable<ICommand> ProcessKeyString(string keyString);
        KeyInputMode Mode { get; }
        void SetMode(KeyInputMode mode);
    }
}