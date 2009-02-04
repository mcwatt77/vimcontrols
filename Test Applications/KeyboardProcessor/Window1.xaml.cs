using System;
using System.Windows;
using System.Windows.Input;
using ActionDictionary;
using Utility.Core;

namespace KeyboardProcessor
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1
    {
        public Window1()
        {
            InitializeComponent();
        }

        private static readonly MessageDictionary _mDict = new MessageDictionary();

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape) ClearText();
            ProcessKey(e);
            base.OnKeyDown(e);
        }

        private void ClearText()
        {
            EDIT_TYPING.Text = "";
            EDIT_OUTPUT.Text = "";
        }

        private void ProcessKey(KeyEventArgs e)
        {
            try
            {

                var messages = _mDict.ProcessKey(e.Key);
                messages.Do(msg => EDIT_OUTPUT.Text += "<<" + msg + ">>");
                //Do something with the messages!

                EDIT_TYPING.Text += "<" + e.Key + ">";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                //reset everything
            }
        }
    }
}