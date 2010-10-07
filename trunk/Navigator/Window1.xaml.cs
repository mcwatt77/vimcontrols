using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Navigator.Containers;
using Navigator.Path;
using Navigator.Path.Agile;
using Navigator.Path.Hd;
using Navigator.Path.Jobs;
using Navigator.Path.KevinBacon;
using Navigator.Path.Notes;
using Navigator.Path.Schemas;
using Navigator.Path.Wow;
using Navigator.UI;
using Navigator.UI.Attributes;
using VIControls.Commands;
using VIControls.Commands.Interfaces;

namespace Navigator
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : IHasModes, IMessageable, ISearchEdit
    {
        private readonly IContainer _container = new Container();
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
                _container.Register(typeof(AgileDownloader), typeof(AgileDownloader), ContainerRegisterType.Singleton);
                _container.Register(typeof(MessageBroadcaster), typeof(MessageBroadcaster), ContainerRegisterType.Instance);
                _container.Register(typeof (IUIElementFactory), typeof (UIElementFactory), ContainerRegisterType.Instance);
                _container.Register(typeof (PathNavigator), typeof (PathNavigator), ContainerRegisterType.Instance);
                _container.RegisterInstance(typeof(IHasInsertMode), this);
                _container.RegisterInstance(typeof(IHasNormalMode), this);

                _port = new UIPort(_container, null, MainStack);
                _container.RegisterInstance(typeof (IUIPort), _port);

                RegisterPathNavigator();
                _container.Register(typeof (JobProgress), typeof (JobProgress), ContainerRegisterType.Singleton);

                _container.Get<IUIElementFactory>();

                var navigable = _container.Get<PathCollection>(
                    _container.GetOrDefault<JobList>(),
                    _container.GetOrDefault<AgileLinks>(),
                    //                                              _container.GetOrDefault<RssPath>("http://www.salon.com/rss/v2/news.rss"),
                    //                                              _container.GetOrDefault<RssPath>("http://rss.slashdot.org/Slashdot/slashdot"),
                    //                                              _container.GetOrDefault<RssPath>(ex => new ExceptionModel(ex), "http://feeds.feedblitz.com/alternet"),
                    //                                              _container.GetOrDefault<RssPath>("http://feeds.huffingtonpost.com/huffingtonpost/raw_feed"),
                    //                                              _container.GetOrDefault<RssPath>("http://blogs.msdn.com/ericlippert/rss.xml"),
                    _container.GetOrDefault<HdPath>(),
                    _container.GetOrDefault<NoteCollection>(),
                    _container.GetOrDefault<SchemaCollection>(),
                    _container.GetOrDefault<KevinBaconCollection>(),
                    _container.GetOrDefault<SpellCollectionPath>(),
                    _container.GetOrDefault<TalentCollectionPath>());

                _port.Navigate(navigable);
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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            var message = KeyMap.GetMessage(_commandMode, e.Key);
            if (message == null) return;

            message.Invoke(this);
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

        public object Execute(Message message)
        {
            return message.Invoke(_port);
        }

        public void Output(char c)
        {
            StatusLine.Text += c;
        }

        public void ExecuteSearch()
        {
            //Save into local variable to avoid unintentionally modifying the closure
            var text = StatusLine.Text;
            var message = Message.Build<IIsSearchable>(search => search.CommitSearch(text));

            MainGrid.RowDefinitions.ElementAt(1).Height = new GridLength(0);
            StatusLine.Text = "";
            _commandMode = CommandMode.Normal;

            message.Invoke(this);
        }

        public void Backspace()
        {
            if (StatusLine.Text.Length == 0) return;

            StatusLine.Text = StatusLine.Text.Substring(0, StatusLine.Text.Length - 1);
        }

        public void SetCursor(int row, int column)
        {
        }
    }
}