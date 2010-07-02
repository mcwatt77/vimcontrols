using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Navigator.Containers;
using Navigator.Path;
using Navigator.Path.Hd;
using Navigator.Path.Rss;
using Navigator.UI;
using Navigator.UI.Attributes;

namespace Navigator
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : IUIPort
    {
        private IUIElementFactory _uiElementFactory;
        private StackPanelWrapper _stackPanelWrapper;
        private readonly IContainer _container = new Container();
        private INavigable _navigable;
        private IVerticallyNavigable _verticallyNavigable;

        public Window1()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _stackPanelWrapper = new StackPanelWrapper(MainStack, false);

            try
            {
                _container.Register(typeof (IUIElementFactory), typeof (UIElementFactory), ContainerRegisterType.Singleton);
                _container.Register(typeof (PathNavigator), typeof (PathNavigator), ContainerRegisterType.Instance);
                _container.RegisterInstance(typeof (IUIPort), this);
                RegisterPathNavigator();

                _uiElementFactory = _container.Get<IUIElementFactory>();

                _navigable = (INavigable)_container.Get<PathCollection>(
                                                new RssPath("http://www.salon.com/rss/v2/news.rss"),
                                                new RssPath("http://rss.slashdot.org/Slashdot/slashdot"),
                                                new HdPath());
                _verticallyNavigable = (IVerticallyNavigable) _navigable;
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

        public void Navigate(object element)
        {
            MainStack.Children.Clear();

            var uiElement = _uiElementFactory.GetUIElement(element);
            uiElement.Render(_stackPanelWrapper);
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
                    _navigable.Navigate();
                    break;
                case Key.Back:
                    _navigable.Back();
                    break;
            }
        }
    }
}
