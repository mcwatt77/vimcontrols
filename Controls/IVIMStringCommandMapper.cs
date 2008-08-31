using System;
using System.Collections.Generic;
using VIMControls.Contracts;

namespace VIMControls.Controls
{
    public interface IVIMStringCommandMapper
    {
        IVIMAction MapCommand(string cmd);
    }

    public class VIMStringCommandMapper : IVIMStringCommandMapper
    {
        private readonly Dictionary<string, Delegate> _commandLookup = new Dictionary<string, Delegate>
                                                                  {
                                                                      {"delete", (Action<IVIMPersistable>)(c => c.Delete())},
                                                                      {"reset", (Action<IVIMController>)(c => c.ResetInput())}
                                                                  };
        public IVIMAction MapCommand(string cmd)
        {
            //should make tests that says it doesn't throw an exception
            if (!_commandLookup.ContainsKey(cmd)) throw new Exception("Command '" + cmd + "' not found");

            return new VIMAction(_commandLookup[cmd]);
        }
    }
}