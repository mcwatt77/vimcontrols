using AcceptanceTests.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;

namespace AcceptanceTests.InputEditorTests
{
    public class TextEditor
    {
        private MockRepository _repository;
        private IApplication _app;

        [SetUp]
        public void Setup()
        {
            _repository = new MockRepository();
            _app = Concepts.SetupApplication(_repository);

            NavigateToNotes(_app);
        }

        public static void NavigateToNotes(IApplication app)
        {
            app.ProcessKeyString("/notes<cr>l<cr>");
            Assert.IsInstanceOfType(typeof(ITextEditor), app.CurrentView);
        }

        private void TestTextString(string input, string output)
        {
            _app.ProcessKeyString(input);

            var editor = (ITextEditor) _app.CurrentView;
            Assert.AreEqual(output, editor.Text);
        }

        [Test]
        public void TextEditorStartsInNormalMode()
        {
            Assert.AreEqual(InputMode.Normal, _app.Mode);
        }

        [Test]
        public void ApplicationModeGetsSetFromTextEditor()
        {
            _app.ProcessKeyString("a");
            Assert.AreEqual(InputMode.TextInsert, _app.Mode);
        }

        [Test]
        public void CanSaveAndReadNotes()
        {
            var testString = "Hello foobar!";
            _app.ProcessKeyString("i" + testString + ":w notetest<cr>:q");

            NavigateToNotes(_app);
            _app.ProcessKeyString(":e notetest<cr>");

            var editor = (ITextEditor) _app.CurrentView;
            Assert.AreEqual(testString, editor.Text);
        }

        [Test]
        public void VimCommandsWorkAsExpected()
        {
            TestTextString("aa", "a");
            TestTextString("a<cr><cr><cr><bk><bk><bk>", "");
            TestTextString("aa<cr>p", "a\r\np");
            TestTextString("aa<cr>b<bk><bk>c", "ac");
            TestTextString("aaa<cr>bb<bk><bk><bk><bk><bk>c", "c");
            TestTextString("aabc<esc>hid<esc>ae", "adebc");
        }

        [Test]
        public void QpushesEditorContentToStack()
        {
            var testString = "Hello foobar!";
            _app.ProcessKeyString("i" + testString + "<esc>Q");

            Assert.AreEqual(InputMode.Stack, _app.Mode);
            Assert.IsInstanceOfType(typeof(IStackView), _app.CurrentView);

            CommandStack.VerifyStackString(_app, testString);
        }
    }
}