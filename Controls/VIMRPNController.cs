using System;
using System.Collections.Generic;
using System.Linq;
using VIMControls.Controls;

namespace VIMControls.Controls
{
    public class VIMRPNController : VIMTextControl, IVIMCommandController
    {
        private readonly Stack<string> _stack = new Stack<string>();
        private readonly Dictionary<char, Action> _operators;

        private string LastTextLine
        {
            get
            {
                return _textData[_textData.Length - 1].Text;
            }
            set
            {
                _textData[_textData.Length - 1].Text = value;
            }
        }

        public VIMRPNController()
        {
            _operators = new Dictionary<char, Action>
                             {
                                 {'+', Add},
                                 {'*', Multiply},
                                 {'-', Subtract},
                                 {'/', Divide}
                             };
        }

        public void EnterCommandMode()
        {
            LastTextLine = "";
        }

        public void InfoCharacter(char c)
        {
        }

        public void CommandCharacter(char c)
        {
            if (_operators.ContainsKey(c))
            {
                Push();
                _operators[c]();
                return;
            }
            LastTextLine += c;
        }

        private void Push()
        {
            if (LastTextLine.Length > 0)
                _stack.Push(LastTextLine);
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

            LastTextLine = "";
        }

        public void Execute()
        {
            var cmd = LastTextLine;
            Push();
            Process(cmd);
        }

        public void CommandBackspace()
        {
            var length = LastTextLine.Length;
            if (length > 0)
                LastTextLine = LastTextLine.Remove(length - 1);
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

        private void Push(object o)
        {
            _stack.Push(o.ToString());
            UpdateDisplayFromStack();
        }

        private void Add()
        {
            var i0 = Pop<int>();
            var i1 = Pop<int>();
            Push(i0 + i1);
        }

        private void Multiply()
        {
            var i0 = Pop<int>();
            var i1 = Pop<int>();
            Push(i0*i1);
        }

        private void Divide()
        {
            var i0 = Pop<int>();
            var i1 = Pop<int>();
            Push(i1 / i0);
        }

        private void Subtract()
        {
            var i0 = Pop<int>();
            var i1 = Pop<int>();
            Push(i1 - i0);
        }

        private void Process(string cmd)
        {
            if (String.IsNullOrEmpty(cmd)) return;
            if (cmd == "add") Add();
        }
    }
}