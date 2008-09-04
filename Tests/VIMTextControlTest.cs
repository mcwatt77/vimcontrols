using System;
using System.Linq;
using NUnit.Framework;
using VIMControls;
using VIMControls.Contracts;
using VIMControls.Controls.Misc;

namespace Tests
{
    [TestFixture]
    public class VIMTextControlTest
    {
        [Ignore]
        [Test]
        public void IDontDirectlyImplementInAnyCapacityAllOfTheMethodsImResponsibleForButITellObjectCreatorWhoDoes()
        {
            //must call IVIMServiceRegistry(xxx, IVIMCharacterController); etc for each interface in the ITextInputProvider
            Assert.Fail();
        }

        [Test]
        public void IWantToBeThereWhenTheObjectCreatorNeedsATextInputProviderButMyDataObjectShouldGetTextNotMe()
        {
            Assert.AreEqual(typeof (VIMTextControl), Services.Locate<ITextInputProvider>()().GetType(), "Should find VIMTextControl when looking for an ITextInputProvider.");
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
