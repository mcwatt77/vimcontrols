﻿using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using VIMControls;
using VIMControls.Input;
using VIMControls.Interfaces;
using VIMControls.Interfaces.Framework;
using VIMControls.Interfaces.Input;

namespace AcceptanceTests
{
    [TestFixture]
    public class Concepts
    {
        private MockRepository _repository;

        [SetUp]
        public void Setup()
        {
            _repository = new MockRepository();
        }

        public static IApplication SetupApplication(MockRepository repository, IFactory<IView> viewFactory, IFactory<ICommand> cmdFactory)
        {
            var app = new VIMApplication();
            var container = repository.StrictMock<IContainer>();
            var elementFactory = repository.StrictMock<IFactory<IBrowseElement>>();
            var list = new LinkedList<IBrowseElement>();

            var keyGen = new KeyCommandGenerator(cmdFactory);
            keyGen.InitializeKeysToCommands();
            container.Expect(a => a.Get<IKeyCommandGenerator>()).Return(keyGen);
            container.Expect(a => a.Get<IFactory<IBrowseElement>>()).Return(elementFactory);
            elementFactory.Expect(a => a.Create("Command Stack")).Return(new CreateBrowseElement{DisplayName = "Command Stack"});
            elementFactory.Expect(a => a.Create("Files")).Return(new CreateBrowseElement{DisplayName = "Files"});
            elementFactory.Expect(a => a.Create("Audio")).Return(new CreateBrowseElement{DisplayName = "Audio"});
            elementFactory.Expect(a => a.Create("Movies")).Return(new CreateBrowseElement{DisplayName = "Movies"});
            elementFactory.Expect(a => a.Create("Objects")).Return(new CreateBrowseElement{DisplayName = "Objects"});
            elementFactory.Expect(a => a.Create("Notes")).Return(new CreateBrowseElement{DisplayName = "Notes"});
            container.Expect(a => a.Get<ILinkedList<IBrowseElement>>()).Return(list);
            container.Expect(a => a.Get<IBrowser>(list)).Return(new LinkedListBrowser(app, list));
            container.Expect(a => a.Get<IFactory<IView>>()).Return(viewFactory);
            repository.ReplayAll();

            app.Initialize(container);

            return app;
        }

        [Test]
        public void WhenIInitializeTheApplicationIGetAnApplicationList()
        {
            var app = SetupApplication(_repository, null, new TestCommandFactory());

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

            _repository.VerifyAll();
        }
    }
}