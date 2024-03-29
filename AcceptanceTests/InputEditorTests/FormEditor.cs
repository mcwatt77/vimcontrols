﻿using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using VIMControls.Interfaces;
using VIMControls.Interfaces.Framework;

namespace AcceptanceTests.InputEditorTests
{
    [TestFixture]
    public class FormEditor
    {
        private MockRepository _repository;
        private IApplication _app;

        [SetUp]
        public void Setup()
        {
            _repository = new MockRepository();
            _app = Concepts.SetupApplication(_repository, null, new TestCommandFactory());

            NavigateToFormEditor(_app);
        }

        public static void NavigateToFormEditor(IApplication app)
        {
            app.KeyGen.ProcessKeyString("/objects<cr>l<cr>");
            Assert.IsInstanceOfType(typeof(IFormEditor), app.CurrentView);
        }

        [Test]
        public void CanCreateANewForm()
        {
            _app.KeyGen.ProcessKeyString("title Now is the winter of our discount tent!<cr>");
            _app.KeyGen.ProcessKeyString("color re<cr>");
            _app.KeyGen.ProcessKeyString("<esc>:w sa<cr>:q");

            NavigateToFormEditor(_app);

            _app.KeyGen.ProcessKeyString(":e sa<cr>");

            var formEditor = (IFormEditor) _app.CurrentView;
            var keyPair = formEditor.Data.Skip(1).First();

            Assert.IsInstanceOfType(typeof(IStringExpression), keyPair.Value);

            var stringExpression = (IStringExpression) keyPair.Value;
            Assert.AreEqual("red", stringExpression.Value);

            Assert.Fail();
        }
    }
}
