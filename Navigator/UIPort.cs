using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Navigator.Containers;
using Navigator.UI;
using VIControls.Commands.Interfaces;

namespace Navigator
{
    public class UIPort : IUIPort
    {
        private readonly IContainer _container;
        private readonly StackPanel _mainStack;
        private readonly Action<object> _fnUpdateObject;
        private IUIElement _activeElement;
        private object _navObject;
        private readonly StackPanelWrapper _stackPanelWrapper;
        private readonly Stack<History> _history = new Stack<History>();
        private readonly IHasNormalMode _normalMode;

        public UIPort(IContainer container, ScrollViewer scrollViewer, StackPanel mainStack, Action<object> fnUpdateObject)
        {
            _container = container;
            _normalMode = _container.Get<IHasNormalMode>();

            _mainStack = mainStack;
            _fnUpdateObject = fnUpdateObject;

            _stackPanelWrapper = new StackPanelWrapper(mainStack, scrollViewer, false);
        }

        public void Back()
        {
            if (_history.Count < 2) return;

            _history.Pop();
            var history = _history.Pop();
            Navigate(history.Element);
        }

        public void Navigate(object navObject)
        {
            _normalMode.EnterNormalMode();

            _history.Push(new History(navObject, 0));

            _navObject = navObject;

            _mainStack.Children.Clear();

            _activeElement = _container.Get<IUIElementFactory>().GetUIElement(navObject);

            _fnUpdateObject(navObject);

            _activeElement.Render(_stackPanelWrapper);
        }

        public object ActiveModel
        {
            get { return _navObject; }
        }

        public IUIElement ActiveUIElement
        {
            get { return _activeElement; }
        }

        private class History
        {
            public History(object element, int index)
            {
                Element = element;
                Index = index;
            }
            public object Element { get; private set; }
            //TODO: Index doesn't work here.  I need to replace this with some sort of Path memory, much like the status updates will work
            public int Index { get; private set; }
        }
    }
}