using System.Linq;
using AcceptanceTests.Interfaces;
using NUnit.Framework;
using Rhino.Mocks;

namespace AcceptanceTests
{
    public class Concepts
    {
        private MockRepository _repository;

        [SetUp]
        public void Setup()
        {
            _repository = new MockRepository();
        }

        public static IApplication SetupApplication(MockRepository repository)
        {
            var app = repository.DynamicMock<IApplication>();
            app.Initialize();

            return app;
        }

        [Test]
        public void WhenIInitializeTheApplicationIGetAnApplicationList()
        {
            var app = SetupApplication(_repository);

            Assert.IsInstanceOfType(typeof(IBrowser), app.CurrentView);
            var browser = (IBrowser) app.CurrentView;
            var displayNames = browser.Elements
                .Select(elem => elem.DisplayName);

            Assert.IsTrue(displayNames.Contains("Notes"));
            Assert.IsTrue(displayNames.Contains("Objects"));
            Assert.IsTrue(displayNames.Contains("Movies"));
            Assert.IsTrue(displayNames.Contains("Audio"));
            Assert.IsTrue(displayNames.Contains("Files"));
            Assert.IsTrue(displayNames.Contains("Command Stack"));
        }
    }
}