using System;
using System.Collections.Generic;
using System.Linq;
using Navigator.UI;
using Navigator.UI.Attributes;

namespace Navigator
{
    public class PathNavigator : IVerticallyNavigable, INavigable, IMessageable
    {
        public PathNavigator(object modelElement, IUIPort port, IUIElementFactory elementFactory)
        {
            _modelElement = modelElement;
            _fnNavigate = port.Navigate;

            var modelChildren = _modelElement as IModelChildren;

            foreach (var child in modelChildren == null ? new object[] { } : modelChildren.Children)
            {
                if (child == null) continue;

                _uiElementLookup[child] = elementFactory.GetUIElement(child);
            }

            _uiElementFactory = new UIElementFactory(elementFactory, _uiElementLookup);
            ((Navigator.UIElementFactory) elementFactory).FactoryForChildren = _uiElementFactory;

            MoveVertically(_index);

            port.Navigate(modelElement);
        }

        private readonly Stack<History> _history = new Stack<History>();
        private object _modelElement;
        private readonly Action<object> _fnNavigate;
        private int _index;
        private readonly Dictionary<object, IUIElement> _uiElementLookup = new Dictionary<object, IUIElement>();
        private readonly UIElementFactory _uiElementFactory;

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

        public IUIElementFactory ElementFactory
        {
            get
            {
                return _uiElementFactory;
            }
        }

        private class UIElementFactory : IUIElementFactory
        {
            private readonly IUIElementFactory _uiElementFactory;
            private readonly Dictionary<object, IUIElement> _dictionary;

            public UIElementFactory(IUIElementFactory uiElementFactory, Dictionary<object, IUIElement> dictionary)
            {
                _uiElementFactory = uiElementFactory;
                _dictionary = dictionary;
            }

            public IUIElement GetUIElement(object modelElement)
            {
                if (!_dictionary.ContainsKey(modelElement))
                    _dictionary[modelElement] = _uiElementFactory.GetUIElement(modelElement);
                return _dictionary[modelElement];
            }
        }

        private IUIElement GetUIElement(object modelElement)
        {
            return _uiElementFactory.GetUIElement(modelElement);
        }

        public void MoveVertically(int spaces)
        {
            var modelChildren = _modelElement as IModelChildren;
            var children = modelChildren == null ? new object[] {} : modelChildren.Children.Where(child => child != null);

            var element = children.ElementAtOrDefault(_index);
            var nextElement = children.ElementAtOrDefault(_index + spaces);
            if (element == null || nextElement == null) return;

            GetUIElement(element).SetFocus(false);
            _index += spaces;
            GetUIElement(nextElement).SetFocus(true);
        }

        //TODO: Modify this method to call GetCurrentChild().Navigate() instead of doing what it does here.
        public void NavigateToCurrentChild()
        {
            var currentChild = GetCurrentChild();

            var uiElement = _uiElementFactory.GetUIElement(currentChild) as INavigableObject;
            if (uiElement != null)
            {
                uiElement.Navigate();
                return;
            }

            _history.Push(new History(_modelElement, _index));

            UpdateView(currentChild);
        }

        public void Navigate()
        {
            UpdateView(_modelElement);
        }

        private object GetCurrentChild()
        {
            var modelChildren = _modelElement as IModelChildren;
            var children = modelChildren == null ? new object[] {} : modelChildren.Children.Where(child => child != null);

            return children.ElementAtOrDefault(_index);
        }

        public void Back()
        {
            if (_history.Count == 0) return;

            var history = _history.Pop();
            _modelElement = history.Element;
            _index = history.Index;

            UpdateView(_modelElement);
        }

        private void UpdateView(object element)
        {
            if (element == null) return;

            _uiElementLookup.Clear();

            var modelChildren = _modelElement as IModelChildren;
            var children = modelChildren == null ? new object[] {} : modelChildren.Children.Where(child => child != null);

            foreach (var child in children)
                _uiElementLookup[child] = _uiElementFactory.GetUIElement(child);

            GetUIElement(element).SetFocus(false);

            _fnNavigate(element);
            _modelElement = element;

            _index = 0;
            MoveVertically(_index);
        }

        public void Execute(Message message)
        {
/*            var uiElement = _uiElementFactory.GetUIElement(_modelElement);
            if (message.CanHandle(uiElement))
                message.Invoke(uiElement);
            else*/
                message.Delegate.DynamicInvoke(this);

/*            var child = GetCurrentChild();
            if (message.CanHandle(child)) message.Invoke(child);
            else message.Delegate.DynamicInvoke(this);*/
        }
    }
}