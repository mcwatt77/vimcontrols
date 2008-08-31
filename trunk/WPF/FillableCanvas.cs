using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VIMControls.Controls;

namespace VIMControls
{
    public class FillableCanvas : Canvas
    {
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            Children
                .OfType<ICanvasChild>()
                .Do(child =>
                        {
                            child.Height = ActualHeight;
                            child.Width = ActualWidth;
                        });
        }
    }
}