using System;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using VIMControls;
using VIMControls.Interfaces;
using VIMControls.Interfaces.Framework;

namespace AcceptanceTests.InputEditorTests
{
    [TestFixture]
    public class TextEditor
    {
        private MockRepository _repository;
        private IApplication _app;
        private ITextEditor _textEditor;
        private TestCommandFactory _cmdFactory;

        [SetUp]
        public void Setup()
        {
            _repository = new MockRepository();

            var viewFactory = _repository.StrictMock<IFactory<IView>>();
            _textEditor = _repository.StrictMock<ITextEditor>();
            viewFactory.Expect(a => a.Create(String.Empty)).Return(_textEditor);
            _app = Concepts.SetupApplication(_repository, viewFactory, _cmdFactory = new TestCommandFactory());

            NavigateToNotes(_app);
        }

        public void NavigateToNotes(IApplication app)
        {
            app.KeyGen.ProcessKeyString("/notes<cr>l<cr>").Do(a => a.Invoke(app));

            var execedCmds = _cmdFactory.RequestedExpressions.Select(expression => expression.ToString()).ToList();
            Assert.AreEqual("a => a.SetMode(Search)", execedCmds[0]);
            Assert.IsInstanceOfType(typeof(ITextEditor), app.CurrentView);
        }

        [Test]
        public void TestModeChange()
        {
            _textEditor.Expect(a => a.Text).Return("");
            _repository.ReplayAll();

            TestTextString("i","");
        }

        private void TestTextString(string input, string output)
        {
            _app.KeyGen.ProcessKeyString(input).Do(a => a.Invoke(_app));

            var editor = (ITextEditor) _app.CurrentView;
            Assert.AreEqual(output, editor.Text);
        }

        [Test]
        public void CanSaveAndReadNotes()
        {
            var testString = "Hello foobar!";
            _app.KeyGen.ProcessKeyString("i" + testString + ":w notetest<cr>:q").Do(a => a.Invoke(_app));

            NavigateToNotes(_app);
            _app.KeyGen.ProcessKeyString(":e notetest<cr>");

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
            _app.KeyGen.ProcessKeyString("i" + testString + "<esc>Q");

            Assert.IsInstanceOfType(typeof(IStackView), _app.CurrentView);

            CommandStack.VerifyStackString(_app, testString);
        }
    }
}