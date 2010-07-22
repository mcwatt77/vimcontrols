using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Navigator.Containers;
using Navigator.Path;
using Navigator.Path.Hd;
using Navigator.Path.Jobs;
using Navigator.Path.KevinBacon;
using Navigator.Path.Notes;
using Navigator.Path.Schemas;
using Navigator.UI;
using Navigator.UI.Attributes;
using VIControls.Commands;
using VIControls.Commands.Interfaces;

namespace Navigator
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : IHasSearchMode, IHasInsertMode, IHasNormalMode
    {
        private readonly IContainer _container = new Container();
        private INavigable _navigable;
        private UIPort _port;
        private CommandMode _commandMode = CommandMode.Normal;

        public Window1()
        {
            InitializeComponent();
        }

        //TODO: Get most of the code out of this class into other classes.  I'm going to think about how to re-architect everything if I need to.  It's soo close...
        //TODO: Plot out the important objects and what "information" is getting passed between classes.

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _container.Register(typeof(MessageBroadcaster), typeof(MessageBroadcaster), ContainerRegisterType.Instance);
                _container.Register(typeof (IUIElementFactory), typeof (UIElementFactory), ContainerRegisterType.Instance);
                _container.Register(typeof (PathNavigator), typeof (PathNavigator), ContainerRegisterType.Instance);
                _container.RegisterInstance(typeof(IHasInsertMode), this);
                _container.RegisterInstance(typeof(IHasNormalMode), this);

                _port = new UIPort(_container, null, MainStack, UpdateObject);
                //TODO: Requires that IHasNormalMode has been registered.  Make this a meaningful error.
                _container.RegisterInstance(typeof (IUIPort), _port);

                RegisterPathNavigator();
                _container.Register(typeof (JobProgress), typeof (JobProgress), ContainerRegisterType.Singleton);

                _container.Get<IUIElementFactory>();

                _navigable = (INavigable) _container.Get<PathCollection>(
                                              _container.GetOrDefault<JobList>(),
//                                              _container.GetOrDefault<RssPath>("http://www.salon.com/rss/v2/news.rss"),
//                                              _container.GetOrDefault<RssPath>("http://rss.slashdot.org/Slashdot/slashdot"),
//                                              _container.GetOrDefault<RssPath>(ex => new ExceptionModel(ex), "http://feeds.feedblitz.com/alternet"),
//                                              _container.GetOrDefault<RssPath>("http://feeds.huffingtonpost.com/huffingtonpost/raw_feed"),
//                                              _container.GetOrDefault<RssPath>("http://blogs.msdn.com/ericlippert/rss.xml"),
                                              _container.GetOrDefault<HdPath>(),
                                              _container.GetOrDefault<NoteCollection>(),
                                              _container.GetOrDefault<SchemaCollection>(),
                                              _container.GetOrDefault<KevinBaconCollection>());

                var initialize = _navigable;
                initialize.Navigate();
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
            _navigable = navObject as INavigable;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            var message = KeyMap.GetMessage(_commandMode, e.Key);
            if (message != null)
            {
                message.Invoke(_navigable);
            }

            switch (e.Key)
            {
                case Key.OemQuestion:
                    message = Message.Build<IHasSearchMode>(x => x.EnterSearchMode());
                    message.Invoke(this);
                    return;
                case Key.Back:
                    message = Message.Build<IUIPort>(x => x.Back());
                    message.Invoke(_port);
                    return;
                default:
                    return;
            }
        }

        public void EnterSearchMode()
        {
            MainGrid.RowDefinitions.ElementAt(1).Height = new GridLength(20);
            StatusLine.Text = "/";
            _commandMode = CommandMode.Search;
        }

        public void EnterInsertMode()
        {
            _commandMode = CommandMode.Insert;
        }

        public void EnterNormalMode()
        {
            _commandMode = CommandMode.Normal;
        }
    }
}