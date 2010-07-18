using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Navigator.Containers;
using Navigator.Path;
using Navigator.Path.Hd;
using Navigator.Path.Jobs;
using Navigator.Path.Notes;
using Navigator.Path.Schemas;
using Navigator.UI;
using Navigator.UI.Attributes;

namespace Navigator
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : IUIPort
    {
        private StackPanelWrapper _stackPanelWrapper;
        private readonly IContainer _container = new Container();
        private INavigable _navigable;
        private INavigableHistory _navigableHistory;
        private IVerticallyNavigable _verticallyNavigable;
        private IUIElement _activeElement;

        public Window1()
        {
            InitializeComponent();
        }

        public INavigableHistory NavigableHistory
        {
            get { return _navigableHistory; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _stackPanelWrapper = new StackPanelWrapper(MainStack, false);

            try
            {
                _container.Register(typeof (IUIElementFactory), typeof (UIElementFactory), ContainerRegisterType.Instance);
                _container.Register(typeof (PathNavigator), typeof (PathNavigator), ContainerRegisterType.Instance);
                _container.RegisterInstance(typeof (IUIPort), this);
                RegisterPathNavigator();
                _container.Register(typeof (PathNavigator), typeof(NavigatorHistory), ContainerRegisterType.Intercept);
                _container.Register(typeof (JobProgress), typeof (JobProgress), ContainerRegisterType.Singleton);

                _container.Get<IUIElementFactory>();

                _navigable = (INavigable) _container.Get<PathCollection>(
//                                              _container.GetOrDefault<RssPath>("http://www.salon.com/rss/v2/news.rss"),
//                                              _container.GetOrDefault<RssPath>("http://rss.slashdot.org/Slashdot/slashdot"),
//                                              _container.GetOrDefault<RssPath>(ex => new ExceptionModel(ex), "http://feeds.feedblitz.com/alternet"),
//                                              _container.GetOrDefault<RssPath>("http://feeds.huffingtonpost.com/huffingtonpost/raw_feed"),
//                                              _container.GetOrDefault<RssPath>("http://blogs.msdn.com/ericlippert/rss.xml"),
                                              _container.GetOrDefault<HdPath>(),
                                              _container.GetOrDefault<NoteCollection>(),
                                              _container.GetOrDefault<SchemaCollection>(),
                                              _container.GetOrDefault<JobList>());
                _verticallyNavigable = (IVerticallyNavigable) _navigable;
                _navigableHistory = _navigable as INavigableHistory;
//                _navigableHistory = (INavigableHistory) _navigable;
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void RegisterPathNavigator()
        {
            var attributeTypes = GetType().Assembly.GetTypes()
                .Where(type => typeof (IAttribute).IsAssignableFrom(type)
                               && !type.IsInterface
                               && !type.IsAbstract);

            foreach (var type in attributeTypes)
            {
                _container.Register(type, type, ContainerRegisterType.Instance);
                _container.Register(type, typeof (PathNavigator), ContainerRegisterType.Intercept);
            }
        }

        //TODO: History has to be managed from here.  With the new (more appropriate) model of calling navigate on the object I wish to see,
        //they no longer have access to history

        public void Navigate(object navObject)
        {
            MainStack.Children.Clear();

            _activeElement = _container.Get<IUIElementFactory>().GetUIElement(navObject);

            _verticallyNavigable = navObject as IVerticallyNavigable;
            _navigable = navObject as INavigable;
            _navigableHistory = navObject as INavigableHistory;

            _activeElement.Render(_stackPanelWrapper);
        }

        public object ActiveModel
        {
            get { return _navigable; }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.J:
                    _verticallyNavigable.MoveVertically(1);
                    break;
                case Key.K:
                    _verticallyNavigable.MoveVertically(-1);
                    break;
                case Key.Enter:
                    _navigable.NavigateToCurrentChild();
                    break;
                case Key.Back:
//                    _navigableHistory.Back();
                    break;
            }
        }
    }
}
