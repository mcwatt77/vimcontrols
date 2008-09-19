using System.Collections.Generic;
using System.Windows.Input;

namespace VIMControls.Interfaces
{
    public interface IKeyCommandGenerator
    {
        IEnumerable<ICommand> ProcessKey(Key key);
        IEnumerable<ICommand> ProcessKeyString(string keyString);
    }
}
