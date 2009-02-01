using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using KeyStringParser;
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

            Initialize();
        }

        private static readonly Parser _parser = new Parser();
        private static readonly SequencedDictionary<Key, Token> _dict = new SequencedDictionary<Key, Token>();

        public static void Initialize()
        {
            _dict.Add(_parser.Parse("sam"), Token.Sam);
            _dict.Add(_parser.Parse("sa"), Token.Sa);
            _dict.Add(_parser.Parse("spam"), Token.Spam);
            _dict.Add(_parser.Parse("zebra"), Token.Zebra);
            _dict.Add(_parser.Parse("ze"), Token.Ze);
            _dict.Add(_parser.Parse("charlie"), Token.Charlie);
            _dict.Add(_parser.Parse("te"), Token.Te);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Escape) ClearText();
            else ProcessKey(e);
            base.OnKeyDown(e);
        }

        private void ClearText()
        {
            _dict.Flush();
            EDIT_TYPING.Text = "";
            EDIT_OUTPUT.Text = "";
        }

        private void ProcessKey(KeyEventArgs e)
        {
            try
            {
                _dict.Push(e.Key);
                Enumerable
                    .Range(0, _dict.Count())
                    .Select(i => _dict.Pop())
                    .Select(pop => "<" + pop.Value + "(" + pop.Keys.Select(k => k.ToString()).SeparateBy(",") + ")>")
                    .Do(s => EDIT_OUTPUT.Text += s);

                EDIT_TYPING.Text += "<" + e.Key + ">";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                //reset everything
            }
        }
    }

    public enum Token
    {
        Unk, Sam, Sa, Spam, Zebra, Ze, Charlie, Test, Te
    }
}