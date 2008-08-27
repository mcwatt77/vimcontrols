using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VIMControls.Controls
{
    public class VIMMediaControl : Grid, IVIMMotionController, IVIMPositionController
    {
        private readonly IVIMContainer _parent;
        private readonly MediaElement _mediaElement;
        private static readonly MediaElement _singleMediaElement = new MediaElement();

        public VIMMediaControl(IVIMContainer parent)
        {
            _parent = parent;
//            _mediaElement = new MediaElement();
            if (_singleMediaElement.Parent != null)
                ((Grid) _singleMediaElement.Parent).Children.Remove(_singleMediaElement);
            _mediaElement = _singleMediaElement;

            Children.Add(_mediaElement);
            _mediaElement.MediaEnded += VIMMediaControl_MediaEnded;

            var progress = new ProgressBar
                               {
                                   Height = 22,
                                   VerticalAlignment = VerticalAlignment.Bottom,
                                   Background = Brushes.Transparent,
                                   Value = 20
                               };

            var brush = progress.Foreground.Clone();
            brush.Opacity = 0.5;
            progress.Foreground = brush;
//            Children.Add(progress);
        }

        public Uri Source
        {
            get
            {
                return _mediaElement.Source;
            }
            set
            {
                _mediaElement.Source = value;
            }
        }

        void VIMMediaControl_MediaEnded(object sender, RoutedEventArgs e)
        {
            _parent.ResetInput();
        }

        public void MoveVertically(int i)
        {
            _mediaElement.Position = _mediaElement.Position.Add(new TimeSpan(0, 0, 10*i));
        }

        public void MoveHorizontally(int i)
        {
            _mediaElement.Position = _mediaElement.Position.Add(new TimeSpan(0, 0, 10*i));
        }

        public void EndOfLine()
        {
        }

        public void BeginningOfLine()
        {
        }

        public void NextLine()
        {
        }

        public void Move(GridLength horz, GridLength vert)
        {
            if (horz.GridUnitType == GridUnitType.Star)
            {
                if (_mediaElement.NaturalDuration.HasTimeSpan)
                {
                    _mediaElement.Position = new TimeSpan(0, 0, 0, 0, (int)(_mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds * horz.Value));
                }
            }
        }
    }
}