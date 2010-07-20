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
using VIControls.Commands;

namespace Navigator
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1
    {
        private readonly IContainer _container = new Container();
        private INavigable _navigable;
        private IVerticallyNavigable _verticallyNavigable;
        private UIPort _port;

        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _port = new UIPort(_container, null, MainStack, UpdateObject);
                _container.Register(typeof (IUIElementFactory), typeof (UIElementFactory), ContainerRegisterType.Instance);
                _container.Register(typeof (PathNavigator), typeof (PathNavigator), ContainerRegisterType.Instance);
                _container.RegisterInstance(typeof (IUIPort), _port);

                RegisterPathNavigator();
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

                var initialize = (IInitialize) _navigable;
                initialize.Initialize();
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

        public void UpdateObject(object navObject)
        {
            _verticallyNavigable = navObject as IVerticallyNavigable;
            _navigable = navObject as INavigable;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            Message message;
            switch (e.Key)
            {
                case Key.J:
                    message = Message.Build<IVerticallyNavigable>(x => x.MoveVertically(1));
                    break;
                case Key.K:
                    message = Message.Build<IVerticallyNavigable>(x => x.MoveVertically(-1));
                    break;
                case Key.L:
                    message = Message.Build<ICharacterEdit>(x => x.Output('l'));
                    break;
                case Key.Enter:
                    message = Message.Build<INavigable>(x => x.NavigateToCurrentChild());
                    break;
                case Key.Back:
                    _port.Back();
                    return;
                default:
                    return;
            }

            message.Invoke(_navigable);
        }
    }
}
