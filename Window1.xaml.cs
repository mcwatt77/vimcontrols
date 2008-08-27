using System;
using System.Windows;
using System.Windows.Input;

namespace VIMControls
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : IVIMSystemUICommands
    {
        private readonly VIMControlContainer vimControlContainer = new VIMControlContainer();
        public Window1()
        {
            Content = vimControlContainer;
            InitializeComponent();

            vimControlContainer.Navigate("computer");

//            vimControlContainer.Navigate("form");

//            vimControlContainer.Navigate("graph");
        }

        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            try
            {
                base.OnRender(drawingContext);
            }
            catch
            {}
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
            catch(Exception ex)
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
