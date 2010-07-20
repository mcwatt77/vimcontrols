using System;
using System.Collections.Generic;
using System.Linq;
using Navigator.UI;
using Navigator.UI.Attributes;

namespace Navigator
{
    public class PathNavigator : IVerticallyNavigable, INavigable, IMessageable, IUIChildren, IInitialize
    {
        public void Initialize()
        {
            Navigate();
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

        private readonly object _modelElement;
        private readonly IUIPort _port;
        private readonly IUIElementFactory _elementFactory;
        private int _index;

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
            var uiElement = (INavigableObject)GetCurrentModelChild();
            if (uiElement != null) uiElement.Navigate();
        }

        public void Navigate()
        {
            MoveVertically(_index);

            _port.Navigate(_modelElement);
        }

        private object GetCurrentModelChild()
        {
            var modelChildren = _modelElement as IModelChildren;
            if (modelChildren == null) return null;

            return modelChildren.Children.Where(child => child != null).ElementAtOrDefault(_index);
        }

        public object Execute(Message message)
        {
            if (!message.CanHandle(this)) return null;

            //TODO: Figure out what the hell is going on here
            return message.Delegate.DynamicInvoke(this);
        }

        public bool CanHandle(Message message)
        {
            return true;
        }
    }
}