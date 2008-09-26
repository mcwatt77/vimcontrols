using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using VIMControls.Interfaces.Framework;
using VIMControls.Interfaces.Graphing;

namespace AcceptanceTests.InputEditorTests
{
    [TestFixture]
    public class GraphBuilder
    {
        private MockRepository _repository;
        private IApplication _app;

        [SetUp]
        public void Setup()
        {
            _repository = new MockRepository();
            _app = Concepts.SetupApplication(_repository, null, new TestCommandFactory());

            CommandStack.NavigateToCommandStack(_app);
        }

        [Test]
        public void CanAddLineToGraphPool()
        {
            _app.KeyGen.ProcessKeyString("2 3 10 11 line<cr>");
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
            _app.KeyGen.ProcessKeyString("0 0 10 10 line<cr>");
            _app.KeyGen.ProcessKeyString("0 90 0 trot 0 0 -1 tmov tport<cr>");

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
            _app.KeyGen.ProcessKeyString("2 3 10 11 line<cr>");
            var graphView = _app.FindView<IGraphView>();
            var count = graphView.DisplayedObjects
                .OfType<ILine>()
                .Count();

            Assert.AreEqual(1, count);
        }

        private void AddPoint(string x, string y, string name)
        {
            _app.KeyGen.ProcessKeyString(x + " " + y + " " + "pt<cr>" + name + " sto<cr>");
        }

        [Test]
        public void AnchorTwoFunctionsToTheSamePointAndMoveIt()
        {
            _app.KeyGen.ProcessKeyString("2 t sto<cr>");
            AddPoint("0", "0", "p0");
            AddPoint("4", "0", "p1");
            AddPoint("2", "t", "p2");
            _app.KeyGen.ProcessKeyString("p0 p1 line<cr>p1 p2 line<cr>");
            _app.KeyGen.ProcessKeyString("0 -90 0 trot 0 0 -1 tmov tport<cr>");

            var graphView = _app.FindView<IGraphView>();
            var pt0 = (IPoint)graphView.DisplayedObjects.First();
            var pt1 = (IPoint)graphView.DisplayedObjects.Skip(1).First();

            Assert.AreEqual(1.0, pt0.x);
            Assert.AreEqual(3.0, pt1.x);

            _app.KeyGen.ProcessKeyString("1 t sto<cr>");

            var pt = (IPoint)graphView.DisplayedObjects.Single();
            Assert.AreEqual(2.0, pt.x);
        }
    }
}