using System.Collections.Generic;
using System.Linq;
using Navigator.UI;
using Navigator.UI.Attributes;

namespace Navigator
{
    public class NavigatorHistory : INavigableHistory
    {
        public void Back()
        {
        }
    }

    public class PathNavigator : IVerticallyNavigable, INavigable, IMessageable, IUIChildren, IInitialize
    {
        public void Initialize()
        {
            MoveVertically(_index);

            _port.Navigate(_modelElement);
        }

        public PathNavigator(object modelElement, IUIPort port, IUIElementFactory elementFactory)
        {
            _modelElement = modelElement;
            _port = port;
            _elementFactory = elementFactory;

        }

        private IEnumerable<IUIElement> _uiElements;

        public IEnumerable<IUIElement> UIElements
        {
            get
            {
                if (_uiElements == null)
                {
                    var modelChildren = _modelElement as IModelChildren;
                    _uiElements = (modelChildren == null ? new object[] {} : modelChildren.Children)
                        .Where(child => child != null)
                        .Select(child => _elementFactory.GetUIElement(child))
                        .ToArray();
                }
                return _uiElements;
            }
        }

        private readonly Stack<History> _history = new Stack<History>();
        private object _modelElement;
        private readonly IUIPort _port;
        private readonly IUIElementFactory _elementFactory;
        private int _index;

        private class History
        {
            public History(object element, int index)
            {
                Element = element;
                Index = index;
            }
            public object Element { get; private set; }
            public int Index { get; private set; }
        }

        public void MoveVertically(int spaces)
        {
            var element = UIElements.ElementAtOrDefault(_index);
            var nextElement = UIElements.ElementAtOrDefault(_index + spaces);

            if (element == null || nextElement == null) return;

            element.SetFocus(false);
            _index += spaces;
            nextElement.SetFocus(true);
        }

        public void NavigateToCurrentChild()
        {
            var currentChild = GetCurrentChild();

            var uiElement = currentChild as INavigableObject;
            if (uiElement != null)
            {
                uiElement.Navigate();
                return;
            }

            _history.Push(new History(_modelElement, _index));

            var currentModelChild = GetCurrentModelChild() as INavigableObject;

            if (currentModelChild == null) return;

            currentModelChild.Navigate();
        }

        public void Navigate()
        {
            _port.Navigate(_modelElement);
        }

        private object GetCurrentModelChild()
        {
            var modelChildren = _modelElement as IModelChildren;
            if (modelChildren == null) return null;

            return modelChildren.Children.Where(child => child != null).ElementAtOrDefault(_index);
        }

        private IUIElement GetCurrentChild()
        {
            return UIElements.ElementAtOrDefault(_index);
        }

        public void Back()
        {
            if (_history.Count == 0) return;

            var history = _history.Pop();
            _modelElement = history.Element;
            _index = history.Index;


            var currentModelChild = _modelElement as INavigableObject;

            if (currentModelChild == null) return;

            currentModelChild.Navigate();
        }

        public object Execute(Message message)
        {
/*            var uiElement = _uiElementFactory.GetUIElement(_modelElement);
            if (message.CanHandle(uiElement))
                message.Invoke(uiElement);
            else*/
                return message.Delegate.DynamicInvoke(this);

/*            var child = GetCurrentChild();
            if (message.CanHandle(child)) message.Invoke(child);
            else message.Delegate.DynamicInvoke(this);*/
        }
    }
}