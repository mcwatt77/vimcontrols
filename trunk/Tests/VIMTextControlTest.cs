using NUnit.Framework;
using Rhino.Mocks;
using VIMControls;
using VIMControls.Contracts;
using VIMControls.Controls.Misc;

//Test Note

namespace Tests
{
    [TestFixture]
    public class VIMTextControlTest
    {
        private MockRepository repository;

        [SetUp]
        public void Setup()
        {
            var setup = new TestFactorySetup();
            setup.Initialize();
            repository = setup.Repository;
        }

        [Ignore]
        [Test]
        public void IDontDirectlyImplementInAnyCapacityAllOfTheMethodsImResponsibleForButITellObjectCreatorWhoDoes()
        {
            //must call IVIMServiceRegistry(xxx, IVIMCharacterController); etc for each interface in the ITextInputProvider
            Assert.Fail();
        }

        [Test]
        public void testsomething()
        {
            repository.ReplayAll();

            var textInput = Services.Locate<ITextInputProvider>()();
            textInput.Output('a');
            textInput.Output('p');
            textInput.Output('p');
            textInput.Backspace();

            Assert.AreEqual("ap", textInput.Text);

            repository.VerifyAll();
        }

        [Test]
        public void IWantToBeThereWhenTheObjectCreatorNeedsATextInputProviderButMyDataObjectShouldGetTextNotMe()
        {
            repository.ReplayAll();

            Assert.AreEqual(typeof (VIMTextControl), Services.Locate<ITextInputProvider>()().GetType(), "Should find VIMTextControl when looking for an ITextInputProvider.");

            repository.VerifyAll();
        }

        [Ignore]
        [Test]
        public void IWantTheObjectCreatorToKnowThatICanRespondToTextInputAndProvideUIElements()
        {
            Assert.Fail();
        }

        [Ignore]
        [Test]
        public void IWantToKeepTheCollectionOfUIElementsThatDisplaysTheCurrentText()
        {
            Assert.Fail();
        }

        [Ignore]
        [Test]
        public void IWantToTellADifferentObjectToActuallyStoreAndPersistTheDataIDisplay()
        {
            Assert.Fail();
        }

        [Ignore]
        [Test]
        public void WhenSomeoneWantsToKnowWhatTextImDisplayingIWantTheCommandRouterToKnowAnotherObjectDoesThat()
        {
            Assert.Fail();
        }

        [Ignore]
        [Test]
        public void IWantACursorObjectToHandleCursorMotionInsteadOfMe()
        {
            Assert.Fail();
        }

        [Ignore]
        [Test]
        public void IWantTheCursorToAskMeWhenItNeedsToMoveTheViewPortViewOfTheData()
        {
            Assert.Fail();
        }

        public void IMoveTheViewPortDirectlyWhenRequestedToDoSo()
        {
            Assert.Fail();
        }
    }
}
