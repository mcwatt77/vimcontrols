using System.Linq;
using AcceptanceTests.Interfaces;
using AcceptanceTests.Interfaces.Graphing;
using NUnit.Framework;
using Rhino.Mocks;

namespace AcceptanceTests.InputEditorTests
{
    public class CommandStack
    {
        private MockRepository _repository;
        private IApplication _app;

        [SetUp]
        public void Setup()
        {
            _repository = new MockRepository();
            _app = Concepts.SetupApplication(_repository);

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

        [Test]
        public void TwoPlusTwoEquals4()
        {
            _app.ProcessKeyString("2<cr>2+");
            VerifyStackNumeric(_app, 4);
        }

        [Test]
        public void CanAddLineToGraphPool()
        {
            _app.ProcessKeyString("2 3 10 11 line<cr>");
            var graphView = _app.FindView<IGraphView>();
            var line = graphView.DisplayedObjects
                .OfType<ILine>()
                .Single();

            Assert.AreEqual(2.0, line.p0.x);
            Assert.AreEqual(3.0, line.p0.y);
            Assert.AreEqual(10.0, line.p1.x);
            Assert.AreEqual(11.0, line.p1.y);
        }

        [Test]
        public void CanAddLineToRotatedGraphPool()
        {
            _app.ProcessKeyString("0 0 10 10 line<cr>");
            _app.ProcessKeyString("0 90 0 trot 0 0 -1 tmov tport<cr>");

            var graphView = _app.FindView<IGraphView>();
            var pt = graphView.DisplayedObjects
                .OfType<IPoint>()
                .Single();

            Assert.AreEqual(0.0, pt.x);
            Assert.AreEqual(1.0, pt.y);
        }

        [Test]
        public void gclrRemovesAllObjectsFromGraphPool()
        {
            _app.ProcessKeyString("2 3 10 11 line<cr>");
            var graphView = _app.FindView<IGraphView>();
            var count = graphView.DisplayedObjects
                .OfType<ILine>()
                .Count();

            Assert.AreEqual(1, count);
        }
    }
}