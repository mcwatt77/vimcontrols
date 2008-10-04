using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows.Input;
using VIMControls.Input;
using VIMControls.Interfaces;
using VIMControls.Interfaces.Framework;
using VIMControls.Interfaces.Input;
using ICommand=VIMControls.Interfaces.ICommand;

namespace VIMControls.Input
{
    public class KeyCommandGenerator : IKeyCommandGenerator
    {
        private readonly IFactory<ICommand> _commandFactory;
        private KeyModeMap _map;

        public KeyCommandGenerator(IFactory<ICommand> commandFactory)
        {
            _commandFactory = commandFactory;
        }

        public KeyModeMap InitializeKeysToCommands()
        {
            _map = new KeyModeMap();

            var methods = typeof (KeyCommandGenerator)
                .Assembly
                .GetMethodsWithCustomAttribute<KeyMapAttribute>()
                .ToList();

            methods
                .Do(method => method.AttributesOfType<KeyMapAttribute>()
                                  .Do(attr => attr.AddToMap((inputMode, toggle, key, @params) =>
                                                            AddCommand(inputMode, toggle, key, method, @params))));

            ((List<ICommand>)_map[KeyInputMode.Search, KeyToggle.None, Key.Enter])
                .Add(BuildCommand<IKeyModeSwitcher>(a => a.SetMode(KeyInputMode.Normal)));
            _map[KeyInputMode.TextInsert, KeyToggle.None, Key.Escape] = new List<ICommand> {BuildCommand<IKeyModeSwitcher>(a => a.SetMode(KeyInputMode.Normal))};
            _map[KeyInputMode.Normal, KeyToggle.None, Key.I] = new List<ICommand> {BuildCommand<IKeyModeSwitcher>(a => a.SetMode(KeyInputMode.TextInsert))};
            _map[KeyInputMode.Normal, KeyToggle.None, Key.OemQuestion] = new List<ICommand> {BuildCommand<IKeyModeSwitcher>(a => a.SetMode(KeyInputMode.Search))};
            _map[KeyInputMode.Normal, KeyToggle.None, Key.OemSemicolon] = 
                new List<ICommand> {BuildCommand<IKeyModeSwitcher>(a => a.SetMode(KeyInputMode.Command))};
            _map[KeyInputMode.TextInsert, KeyToggle.None, Key.LeftShift] =
                new List<ICommand> {BuildCommand<IKeyModeSwitcher>(a => a.SetToggle(KeyToggle.Shift))};

            return _map;
        }

        private void AddCommand(KeyInputMode inputMode, KeyToggle toggle, Key key, MethodInfo method, params object[] @params)
        {
            var cmds = _map[inputMode, toggle, key];
            var list = cmds == null ? new List<ICommand>() : cmds.ToList();
            list.Add(_commandFactory.Create(method.BuildLambda(@params)));
            _map[inputMode, toggle, key] = list;
        }

        private ICommand BuildCommand<T>(Expression<Action<T>> cmd)
        {
            return _commandFactory.Create(cmd);
        }

        public IEnumerable<ICommand> ProcessKey(Key key)
        {
            var cmds = _map[Mode, KeyToggle.None, key];
            if (cmds == null) throw new Exception("Key " + key + " not found for " + Mode);

            var modeSwitcher = new KeyModeSwitcher(this);
            //todo: this won't work if the list of commands contains a mode switch
            cmds.Do(cmd => cmd.Invoke(modeSwitcher));
            return modeSwitcher.Commands;
        }

        public interface IKeyModeSwitcher : ICommandable
        {
            void SetMode(KeyInputMode mode);
            void SetToggle(KeyToggle toggle);
        }

        public class KeyModeSwitcher : IKeyModeSwitcher
        {
            private readonly KeyCommandGenerator _gen;
            private readonly List<ICommand> _commands = new List<ICommand>();

            public KeyModeSwitcher(KeyCommandGenerator gen)
            {
                _gen = gen;
            }

            public IEnumerable<ICommand> Commands
            {
                get
                {
                    return _commands;
                }
            }

            public void ProcessMissingCommand(ICommand command)
            {
                _commands.Add(command);
            }

            public void SetMode(KeyInputMode mode)
            {
                //todo: want to move IApplication out of this class, can probably do it through events
                _commands.Add(_gen.BuildCommand<IApplication>(a => a.SetMode(mode)));
                _gen.SetMode(mode);
            }

            public void SetToggle(KeyToggle toggle)
            {
                _gen.SetToggle(toggle);
            }
        }

        public IEnumerable<ICommand> ProcessKeyString(string keyString)
        {
            return OldKeyStringParser.ProcessKeyString(keyString)
                .Select(key => ProcessKey(key))
                .Flatten();
        }

        public KeyToggle Toggle { get; private set; }
        public void SetToggle(KeyToggle toggle)
        {
            Toggle = toggle;
        }

        public KeyInputMode Mode { get; private set; }
        public void SetMode(KeyInputMode mode)
        {
            Mode = mode;
        }
    }
}