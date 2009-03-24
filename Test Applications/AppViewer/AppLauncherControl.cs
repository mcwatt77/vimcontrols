using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ActionDictionary;
using ActionDictionary.Interfaces;
using DataProcessors;
using Utility.Core;

namespace AppViewer
{
    public class AppLauncherControl : IAppControl, IAppLauncherUpdate, IMissing
    {
        private readonly MessagePipe _messagePipe;
        private AppLauncherCanvas _canvas;
        private AppLauncher _launcher;

        public AppLauncherControl(MessagePipe messagePipe)
        {
            _messagePipe = messagePipe;
        }

        public UIElement GetControl()
        {
            _launcher = new AppLauncher(_messagePipe, new UpdatePipe(this), GetType().Assembly);
            _canvas = new AppLauncherCanvas(GetTextLines, () => _launcher.HilightIndex);
            return _canvas;
        }

        private IEnumerable<string> GetTextLines()
        {
            return _launcher.AppLines.Select(appLine => appLine.AppText);
        }

        public void Update(IEnumerable<int> indexes)
        {
            _canvas.InvalidateVisual();
        }

        public object ProcessMissingCmd(Message msg)
        {
            return msg.Invoke(_launcher);
        }
    }

    public class AppLauncherCanvas : Canvas
    {
        private readonly Func<IEnumerable<string>> _fnLines;
        private readonly Func<int> _fnHilight;

        public AppLauncherCanvas(Func<IEnumerable<string>> fnLines, Func<int> fnHilight)
        {
            _fnLines = fnLines;
            _fnHilight = fnHilight;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var tf = new Typeface(new FontFamily("Courier New"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            var top = 0.0;

            _fnLines().Do((i,line) => top = RenderLine(line, dc, tf, top, _fnHilight() == i));
        }

        private static double RenderLine(string text, DrawingContext dc, Typeface tf, double top, bool hilight)
        {
            var metrics = new FormattedText(text, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, tf, 15, hilight ? Brushes.Blue : Brushes.Black);
            dc.DrawText(metrics, new Point(0, top));
            return top + metrics.Height;
        }
    }
}