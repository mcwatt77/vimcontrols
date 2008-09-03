using System;
using System.Collections.Generic;
using VIMControls.Contracts;
using VIMControls.Controls.VIMForms;

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
                                                                      {"reset", (Action<IVIMControlContainer>)(c => c.Navigate("."))},
                                                                      {"ftext", (Action<IVIMForm>)(c => c.SetMode(VIMFormConstraint.Multiline))},
                                                                      {"rpn", (Action<IVIMControlContainer>)(c => c.Navigate("rpn"))}
                                                                  };
        public IVIMAction MapCommand(string cmd)
        {
            //should make tests that says it doesn't throw an exception
            if (!_commandLookup.ContainsKey(cmd))
            {
                return new VIMAction((Action<IVIMCommandController>) (c => c.InvalidCommand(cmd)));
            }

            return new VIMAction(_commandLookup[cmd]);
        }
    }
}