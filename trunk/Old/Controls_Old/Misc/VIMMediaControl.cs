using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using VIMControls.Contracts;

namespace VIMControls.Controls.Misc
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
        private DateTime _visibleStartTime = DateTime.Now;
        private bool _alwaysVisible;

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
                                Height = 12,
                                VerticalAlignment = VerticalAlignment.Bottom,
                                Background = Brushes.Transparent,
                                Value = 0,
                                Visibility = Visibility.Hidden,
                                Margin = new Thickness(0, 0, 0, 10)
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

            if (DateTime.Now.Subtract(_visibleStartTime).TotalSeconds > 5 && !_alwaysVisible)
            {
                SetHidden();
                return;
            }

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
                _timeline.CurrentTimeInvalidated += timeline_CurrentTimeInvalidated;
                var clock = _timeline.CreateClock();
                _mediaElement.Clock = clock;

                _story = new Storyboard();
                _story.Children.Add(_timeline);
                _story.Begin(_mediaElement, true);
            }
        }

        private void SetVisible()
        {
            _visibleStartTime = DateTime.Now;

            if (_progress.Visibility == Visibility.Visible) return;

            _progress.Visibility = Visibility.Visible;
        }

        private void SetHidden()
        {
            if (_progress.Visibility == Visibility.Hidden) return;

            _progress.Visibility = Visibility.Hidden;
        }

        void timeline_CurrentTimeInvalidated(object sender, EventArgs e)
        {
            if (_progress.Visibility == Visibility.Visible)
                HeartBeat();
        }

        void VIMMediaControl_MediaEnded(object sender, RoutedEventArgs e)
        {
            MoveHorizontally(1);
        }

        public void MoveVertically(int i)
        {
            SetVisible();

            var ts = _mediaElement.Position.Add(new TimeSpan(0, 0, 10*i));
            if (ts.TotalMilliseconds == 0) return;

            if (ts > _maxTs)
            {
                VIMMediaControl_MediaEnded(null, null);
                return;
            }

            _story.SeekAlignedToLastTick(_mediaElement, ts, TimeSeekOrigin.BeginTime);
            HeartBeat();
        }

        public void MoveHorizontally(int i)
        {
            var fileName = _timeline.Source.ToString().Replace("file:///", "");
            var fileInfo = new FileInfo(fileName);
            var files = fileInfo.Directory.GetFiles().ToList();
            var index = files.FindIndex(file => file.Name == fileInfo.Name);
            index += i;
            if (index < 0) index = files.Count - 1;
            if (index > files.Count - 1) index = 0;
            _parent.Navigate(files[index].FullName);
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
            SetVisible();
            
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
            if (_progress.Visibility == Visibility.Visible)
            {
                SetHidden();
                _alwaysVisible = false;
            }
            else if (_alwaysVisible)
            {
                SetHidden();
                _alwaysVisible = false;
            }
            else
            {
                SetVisible();
                _alwaysVisible = true;
            }
        }

        public void ResetInput()
        {
        }

        public void MissingModeAction(IVIMAction action)
        {
        }

        public void MissingMapping()
        {
        }
    }
}