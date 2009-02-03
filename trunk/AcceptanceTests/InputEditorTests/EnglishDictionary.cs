using System;
using System.Linq;
using KeyStringParser;
using NUnit.Framework;
using Rhino.Mocks;
using VIMControls;
using VIMControls.Interfaces;
using VIMControls.Interfaces.Framework;

namespace AcceptanceTests.InputEditorTests
{
    [TestFixture]
    public class EnglishDictionary
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
            _textEditor = _repository.DynamicMock<ITextEditor>();
            viewFactory.Expect(a => a.Create(String.Empty)).Return(_textEditor);
            _app = Concepts.SetupApplication(_repository, viewFactory, _cmdFactory = new TestCommandFactory());
            _textEditor.Expect(a => a.ProcessMissingCommand(null)).IgnoreArguments();

            NavigateToDictionary(_app);
        }

        private void NavigateToDictionary(IApplication app)
        {
            var parser = new Parser();
            var keys = parser.Parse("/dict<cr>l<cr>");
            var cmds = keys.Select(key => app.KeyGen.ProcessKey(key)).Flatten().ToList();
            //todo: It seems there's a problem.  KeyGen must handle state of shift, normal mode, etc.
            //otherwise, I can't just get a list of commands from ProcessKey
            //But then the app needs to be able to set the mode
            //And I need a command that targets KeyGen

            cmds.Do(a => a.Invoke(app));

//            app.KeyGen.ProcessKeyString("/notes<cr>l<cr>").Do(a => a.Invoke(app));

            var execedCmds = _cmdFactory.RequestedExpressions.Select(expression => expression.ToString()).ToList();
            Assert.AreEqual("a => a.SetMode(Search)", execedCmds[0]);
            Assert.IsInstanceOfType(typeof(ITextEditor), app.CurrentView);
        }

        [Test]
        public void TestInput()
        {
            NavigateToDictionary(_app);

            var parser = new Parser();
            var keys = parser.Parse("iThe is the most common word.<esc>jjiAnd is 3rd.<esc>/that<cr>iThat is 7.<esc><tab>iSpaceMetaphor<cret");
        }
    }
}
