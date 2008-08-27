using System;
using NUnit.Framework;
using VIMControls;

namespace Tests
{
    public interface IUIElement
    {}

    public interface IVIMServiceProvider
    {
        void RegisterServices(IVIMServiceRegistry registry);
    }

    public interface IVIMServiceRegistry
    {
        void Register(IVIMServiceProvider provider, Type forService, params Type[] providedServices);
    }

    public interface ITextInputProvider : IVIMCharacterController, IVIMMotionController, IVIMServiceProvider
    {
        string Text { get; set; }
        IUIElement GetUIElement();
    }

    public class VIMServiceProvider : Attribute
    {
        public VIMServiceProvider(Type type)
        {}
    }

    [VIMServiceProvider(typeof(ITextInputProvider))]
    public class TextInputProvider : IVIMServiceProvider
    {
        public void RegisterServices(IVIMServiceRegistry registry)
        {
            registry.Register(null, typeof(IVIMCharacterController));
        }
    }

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

        [Ignore]
        [Test]
        public void IWantToBeThereWhenTheObjectCreatorNeedsATextInputProviderButMyDataObjectShouldGetTextNotMe()
        {
            Assert.Fail();
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
