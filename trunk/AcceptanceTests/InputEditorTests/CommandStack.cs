using NUnit.Framework;
using Rhino.Mocks;
using VIMControls.Interfaces;
using VIMControls.Interfaces.Framework;

namespace AcceptanceTests.InputEditorTests
{
    [TestFixture]
    public class CommandStack
    {
        private MockRepository _repository;
        private IApplication _app;

        [SetUp]
        public void Setup()
        {
            _repository = new MockRepository();
            _app = Concepts.SetupApplication(_repository, null);

            NavigateToCommandStack(_app);
        }

        public static void NavigateToCommandStack(IApplication app)
        {
            app.ProcessKeyString("k<cr>");
            Assert.IsInstanceOfType(typeof(IStackView), app.CurrentView);
        }

        public static void VerifyStackString(IApplication app, string text)
        {
            var stack = (IStackView) app.CurrentView;

            var expression = stack.Pop();
            Assert.IsInstanceOfType(typeof(IStringExpression), expression);

            var stringExpression = (IStringExpression) expression;
            Assert.AreEqual(stringExpression.Value, text);
        }

        public static void VerifyStackNumeric(IApplication app, double dVal)
        {
            var stack = (IStackView) app.CurrentView;

            var expression = stack.Pop();
            Assert.IsInstanceOfType(typeof(INumericExpression), expression);

            var stringExpression = (INumericExpression) expression;
            Assert.AreEqual(stringExpression.DVal, dVal);
        }

        [Test]
        public void CanEditFileFromStackCmd()
        {
            _app.ProcessKeyString("dir<cr>j<cr>/projects<cr>/accept<cr>concept<cr>");

            Assert.IsInstanceOfType(typeof(ITextEditor), _app.CurrentView);
            var editor = (ITextEditor)_app.CurrentView;

            Assert.Greater(editor.Text.IndexOf("concept"), 0);
        }
    }
}