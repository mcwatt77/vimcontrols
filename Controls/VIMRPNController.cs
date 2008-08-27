using System;
using System.Collections.Generic;
using System.Linq;
using VIMControls.Controls;

namespace VIMControls.Controls
{
    public class VIMRPNController : VIMTextControl, IVIMCommandController
    {
        private readonly Stack<string> _stack = new Stack<string>();

        public void EnterCommandMode()
        {
            _textData[_textData.Length - 1].Text = "";
        }

        public void InfoCharacter(char c)
        {
        }

        public void CommandCharacter(char c)
        {
            if (c == '+')
            {
                Push();
                _textData[_textData.Length - 1].Text = "add";
                Execute();
            }
            else
                _textData[_textData.Length - 1].Text += c;
        }

        private void Push()
        {
            if (_textData[_textData.Length - 1].Text.Length > 0)
                _stack.Push(_textData[_textData.Length - 1].Text);
            UpdateDisplayFromStack();
        }

        private T Pop<T>()
        {
            object ret = _stack.Pop();
            if (typeof(T) == typeof(int))
                ret = Convert.ToInt32(ret);
            return (T) ret;
        }

        private void UpdateDisplayFromStack()
        {
            var idx = 0;
            _stack
                .Take(_textData.Length - 1)
                .Do(s => _textData[_textData.Length - (idx++) - 2].Text = s);

            var imax = _textData.Length - 1 - _stack.Count;
            if (imax > 0)
                Enumerable.Range(0, imax)
                    .Do(i => _textData[i].Text = "");

            _textData[_textData.Length - 1].Text = "";
        }

        public void Execute()
        {
            var cmd = _textData[_textData.Length - 1].Text;
            Push();
            Process(cmd);
        }

        private class Numeric
        {
            public static Numeric FromString(string s)
            {
                return null;
            }

            public bool IsInt { get; private set; }
            public int IntVal { get; private set; }
            public double DoubleVal { get; private set; }

            public static Numeric operator +(Numeric a0, Numeric a1)
            {
                return new Numeric {IsInt = a0.IsInt && a1.IsInt, IntVal = a0.IntVal + a1.IntVal, DoubleVal = a0.DoubleVal + a1.DoubleVal};
            }
        }

        private static Numeric Add(Numeric a0, Numeric a1)
        {
            return a0 + a1;
        }

        private void Process(string cmd)
        {
            if (!String.IsNullOrEmpty(cmd))
            {
                if (cmd == "add")
                {
                    _stack.Pop();
                    var i0 = Pop<int>();
                    var i1 = Pop<int>();
                    _textData[_textData.Length - 1].Text = (i0 + i1).ToString();
                    Push();
                }
            }
        }
    }
}