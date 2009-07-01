namespace Requirements._3d_Sketcher
{
    public class VisualElements
    {
        public void WhenLoadingThePageA2DSketchPadShouldAppearOnTheLeft()
        {}

        public void WhenLoadingThePageA3DViewOfTheObjectsShouldAppearOnTheRight()
        {}
    }

    public class SketchPad2D
    {
        public void WhenPushingTheLeftMouseButtonDownANewPointShouldBeAddedToTheSketch()
        {}

        public void WhenHoldingTheLeftMouseButtonDownTheCurrentControlPointShouldUpdateToTheMousePosition()
        {}

        public void WhenReleasingTheLeftMouseButtonTheCurrentControlPointShouldStopUpdatingToTheMousePosition()
        {}
    }
}