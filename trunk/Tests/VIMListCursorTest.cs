using NUnit.Framework;
using Rhino.Mocks;
using VIMControls;
using VIMControls.Contracts;

namespace Tests
{
    [TestFixture]
    public class VIMListCursorTest
    {
        private IVIMListCursor _cursor;

        private IVIMTextStorage _textStorage;
        private MockRepository _repository;

        [SetUp]
        public void Setup()
        {
            _repository = new MockRepository();
            _textStorage = _repository.StrictMock<IVIMTextStorage>();

            _cursor = ServiceLocator.FindService<IVIMListCursor>(_textStorage)();
        }

        [Ignore]
        [Test]
        public void IKnowWhereIPointInsideTheViewport()
        {
            Assert.Fail();
        }

        [Ignore]
        [Test]
        public void IKnowWhereIPointInsideTheData()
        {
            Assert.Fail();
        }

        [Ignore]
        [Test]
        public void ICanRespondToMovementRequests()
        {
            _textStorage.Expect(e => e.ConvertPosition(null)).Return(new Point{X = 0, Y = 0});
            _repository.ReplayAll();

            _cursor.MoveVertically(1);

            _repository.VerifyAll();
        }

        [Ignore]
        [Test]
        public void ICanReturnAUIElementRelativeToAViewportForRendering()
        {
            Assert.Fail();
        }

        [Ignore]
        [Test]
        public void ICanTellTheViewportToChangeWhereItsLookingInTheData()
        {
            Assert.Fail();
        }

        [Ignore]
        [Test]
        public void ICanAskTheDataStorageObjectToConvertTextPositionToViewportScreenCoords()
        {
            Assert.Fail();
        }
    }
}
