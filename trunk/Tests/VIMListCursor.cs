using System;
using NUnit.Framework;
using VIMControls.Controls;

namespace Tests
{
    [TestFixture]
    public class VIMListCursor
    {
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
            Assert.Fail();
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

    [DependsOn(typeof(IVIMViewport), typeof(IVIMTextStorage))]
    public interface IVIMListCursor : IVIMMotionController, IVIMControl
    {
        IVIMTextDataPosition TextPosition { get; }
        IPoint RenderPosition { get; }
    }

    public class DependsOn : Attribute
    {
        public DependsOn(params Type[] types)
        {}
    }

    public interface IVIMTextStorage
    {
        IPoint ConvertPosition(IVIMTextDataPosition pos);
    }

    public interface IVIMViewport
    {
        void SetDataLine(int line);
    }

    public interface IPoint
    {
        double X { get; }
        double Y { get; }
    }

    public interface IVIMTextDataPosition
    {
        int Column { get; }
        int Line { get; }
    }
}
