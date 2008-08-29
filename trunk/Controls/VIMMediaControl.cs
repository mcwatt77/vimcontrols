using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace VIMControls.Controls
{
    public class VIMMediaControl : Grid, IVIMMotionController, IVIMPositionController
    {
        private readonly IVIMContainer _parent;
        private readonly MediaElement _mediaElement;
        private static readonly MediaElement _singleMediaElement = new MediaElement();
        private readonly ProgressBar _progress;
        private TimeSpan _maxTs;
        private Storyboard _story;
        private MediaTimeline _timeline;

        public VIMMediaControl(IVIMContainer parent)
        {
            _parent = parent;
            if (_singleMediaElement.Parent != null)
                ((Grid) _singleMediaElement.Parent).Children.Remove(_singleMediaElement);
            _mediaElement = _singleMediaElement;

            Children.Add(_mediaElement);
            _mediaElement.MediaEnded += VIMMediaControl_MediaEnded;
            _mediaElement.MediaOpened += _mediaElement_MediaOpened;

            _progress = new ProgressBar
                               {
                                   Height = 18,
                                   VerticalAlignment = VerticalAlignment.Bottom,
                                   Background = Brushes.Transparent,
                                   Value = 0
                               };

            var brush = Brushes.DarkGreen.Clone();
            brush.Opacity = 0.5;
            _progress.Foreground = brush;
            Children.Add(_progress);
        }

        void _mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            _maxTs = _mediaElement.NaturalDuration.TimeSpan;
        }

        public void HeartBeat()
        {
            if (_maxTs == default(TimeSpan)) return;

            var pos = _mediaElement.Position;
            var pct = 100*pos.TotalMilliseconds/_maxTs.TotalMilliseconds;
            _progress.Value = pct;
        }

        public Uri Source
        {
            get
            {
                return _mediaElement.Source;
            }
            set
            {
//                _mediaElement.Source = value;

                _timeline = new MediaTimeline(value);
//                _timeline.CurrentTimeInvalidated += timeline_CurrentTimeInvalidated;
                var clock = _timeline.CreateClock();
                _mediaElement.Clock = clock;

                _story = new Storyboard();
                _story.Children.Add(_timeline);
                _story.Begin(_mediaElement, true);
            }
        }

        void timeline_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            HeartBeat();
        }

        void VIMMediaControl_MediaEnded(object sender, RoutedEventArgs e)
        {
            _parent.ResetInput();
        }

        public void MoveVertically(int i)
        {
            var ts = _mediaElement.Position.Add(new TimeSpan(0, 0, 10*i));
            _story.SeekAlignedToLastTick(_mediaElement, ts, TimeSeekOrigin.BeginTime);
            HeartBeat();
        }

        public void MoveHorizontally(int i)
        {
            var ts = _mediaElement.Position.Add(new TimeSpan(0, 0, 10*i));
            _story.SeekAlignedToLastTick(_mediaElement, ts, TimeSeekOrigin.BeginTime);
            HeartBeat();
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
                    var ts = new TimeSpan(0, 0, 0, 0, (int)(_mediaElement.NaturalDuration.TimeSpan.TotalMilliseconds * horz.Value));
                    _story.SeekAlignedToLastTick(_mediaElement, ts, TimeSeekOrigin.BeginTime);
                }
            }
            HeartBeat();
        }

        public void TogglePositionIndicator()
        {
            HeartBeat();
        }
    }
}