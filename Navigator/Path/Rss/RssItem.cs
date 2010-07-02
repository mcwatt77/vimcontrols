using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Navigator.UI;
using Navigator.UI.Attributes;

namespace Navigator.Path.Rss
{
    public class RssItem : object, ISummaryString, IDescriptionString
    {
        private readonly object _parent;
        private readonly XElement _rssItem;
        private readonly StringSummaryElement _uiElement;
        private readonly XElement _title;
        private readonly XElement _description;

        public RssItem(object parent, XElement rssItem)
        {
            _parent = parent;
            _rssItem = rssItem;
            _title = _rssItem.Elements().First(element => element.Name.LocalName == "title");
            _description = _rssItem.Elements().First(element => element.Name.LocalName == "description");

            _uiElement = new StringSummaryElement(_title.Value, _description.Value);
        }

        public IEnumerable<object> Children
        {
            get { return new[] {_parent}; }
        }

        public IUIElement UIElement
        {
            get { return _uiElement; }
        }

        public string Summary
        {
            get { return _title.Value; }
        }

        public string Description
        {
            get { return _description.Value; }
        }
    }
}