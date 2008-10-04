using System.Collections.Generic;
using System.Windows.Input;
using KeyStringParser;
using NUnit.Framework;

namespace AcceptanceTests
{
    [TestFixture]
    public class ParserTest
    {
        [Test]
        public void CanParseIntoKeys()
        {
            TestString("apple", new[] {Key.A, Key.P, Key.P, Key.L, Key.E});
            TestString("<esc>", new[] {Key.Escape});
            TestString("ia<cr><c-c>", new[] {Key.I, Key.A, Key.Return, Key.LeftCtrl, Key.C});
            TestString("<a-z><space>", new[]
                                           {
                                               Key.A, Key.B, Key.C, Key.D, Key.E, Key.F, Key.G, Key.H, Key.I,
                                               Key.J, Key.K, Key.L, Key.M, Key.N, Key.O, Key.P, Key.Q, Key.R,
                                               Key.S, Key.T, Key.U, Key.V, Key.W, Key.X, Key.Y, Key.Z, Key.Space
                                           });
            TestString("/notes<cr>l<cr>", new[]
                                              {
                                                  Key.OemQuestion, Key.N, Key.O, Key.T, Key.E, Key.S,
                                                  Key.Return, Key.L, Key.Return
                                              });
            TestString("iA!<esc>:w<cr>", new[]
                                             {
                                                 Key.I, Key.LeftShift, Key.A, Key.LeftShift, Key.D1, Key.Escape,
                                                 Key.LeftShift, Key.OemSemicolon, Key.W, Key.Enter
                                             });
        }

        private static void TestString(string input, IEnumerable<Key> expected)
        {
            var parser = new Parser();
            var keys = parser.Parse(input);

            Assert.IsTrue(CompareLists(keys, expected), "Lists did not match for \"" + input + "\"");
        }

        private static bool CompareLists<T>(IEnumerable<T> src, IEnumerable<T> cmp)
        {
            var eSrc = src.GetEnumerator();
            var eCmp = cmp.GetEnumerator();
            var success = false;

            while (eSrc.MoveNext())
            {
                success = eCmp.MoveNext();

                if (!eSrc.Current.Equals(eCmp.Current)) return false;
            }

            return success;
        }
    }
}
