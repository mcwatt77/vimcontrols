using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Navigator.UI.Attributes;

namespace Navigator.Path.Rss
{
    public class RssItem : ISummaryString, IDescriptionString, IHasUrl
    {
        private readonly object _parent;
        private readonly XElement _rssItem;
        private readonly string _title;
        private readonly XElement _description;
        private readonly XElement _link;

        public RssItem(object parent, XElement rssItem)
        {
            _parent = parent;
            _rssItem = rssItem;
            var title = _rssItem.Elements().First(element => element.Name.LocalName == "title");
            _description = _rssItem.Elements().First(element => element.Name.LocalName == "description");
            _link = _rssItem.Elements().First(element => element.Name.LocalName == "link");
            var creator = _rssItem.Elements().First(element => element.Name.LocalName == "creator");

            _title = creator.Value + ": " + title.Value;
        }

        public IEnumerable<object> Children
        {
            get { return new[] {_parent}; }
        }

        public string Summary
        {
            get { return _title; }
        }

        public string Description
        {
            get { return _description.Value; }
        }

        public string Url
        {
            get { return _link.Value; }
        }
    }
}