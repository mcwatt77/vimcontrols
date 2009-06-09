using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using VIMControls.Contracts;

namespace VIMControls
{
    public class Window1 : Window, IVIMSystemUICommands
    {
        private readonly IVIMControlContainer vimControlContainer;
        private readonly IVIMMessageService _msg;

        public Window1()
        {
            Content = vimControlContainer = Services.Locate<IVIMControlContainer>()();
            _msg = Services.Locate<IVIMMessageService>(this)();
            Services.Register<IVIMMessageService>(_msg);

//            vimControlContainer.Navigate("computer");
            vimControlContainer.Navigate("rpn");
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            try
            {
                var keyStates = KeyStates.None;
                keyStates |= e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift) ? KeyStates.Shift : KeyStates.None;
                var actions = KeyMapper.GetVIMCommand(vimControlContainer.Mode, e.Key, keyStates);
                actions.Do(action => action.Invoke(this));
            }
            catch (TargetInvocationException ex)
            {
                MessageBox.Show(ex.InnerException.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            base.OnKeyDown(e);
        }

        public void Maximize()
        {
            WindowState = WindowState.Maximized;
        }

        public void Save()
        {
            vimControlContainer.Save();
        }

        public void About()
        {
            MessageBox.Show("This is like version .05 or something.  Not a lot done yet.  Check back later.");
        }

        public void UpdateTitle()
        {
            Title = vimControlContainer.Mode.ToString();
        }

        public void ResetInput()
        {
            vimControlContainer.ResetInput();
        }

        public void MissingModeAction(IVIMAction action)
        {
            action.Invoke(vimControlContainer);
        }

        public void MissingMapping()
        {
        }
    }
}
