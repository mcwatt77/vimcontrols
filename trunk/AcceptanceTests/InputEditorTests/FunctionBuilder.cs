using NUnit.Framework;
using Rhino.Mocks;
using VIMControls.Interfaces.Framework;

namespace AcceptanceTests.InputEditorTests
{
    [TestFixture]
    public class FunctionBuilder
    {
        private MockRepository _repository;
        private IApplication _app;

        [SetUp]
        public void Setup()
        {
            _repository = new MockRepository();
            _app = Concepts.SetupApplication(_repository, null);

            CommandStack.NavigateToCommandStack(_app);
        }

        [Test]
        public void TestEquations()
        {
            ValidateFunction(_app, "2 2+", 4);
            ValidateFunction(_app, "4 5 x+eval<cr>", 9);
            ValidateFunction(_app, "2 x 3 pow x 2 pow +eval<cr>", 12);
            ValidateFunction(_app, "2 x 3 pow 3 *5+eval<cr>", 29);
            ValidateFunction(_app, "4 t sto x 2 pow t*x 1 fn<cr>eval<cr>", 16);
        }

        public static void ValidateFunction(IApplication app, string fn, double dValExpected)
        {
            app.ProcessKeyString(fn);
            CommandStack.VerifyStackNumeric(app, dValExpected);
        }
    }
}
