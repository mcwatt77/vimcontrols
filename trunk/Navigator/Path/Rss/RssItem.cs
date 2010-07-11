using System;
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
        private readonly string _link;
        private readonly DateTime _date;
        private readonly XElement _originalTitle;
        private readonly XElement _creator;

        public RssItem(object parent, XElement rssItem)
        {
            _parent = parent;
            _rssItem = rssItem;

            _originalTitle = _rssItem.Elements().First(element => element.Name.LocalName == "title");

            _description = _rssItem.Elements().FirstOrDefault(element => element.Name.LocalName == "description") ??
                           _rssItem.Elements().FirstOrDefault(element => element.Name.LocalName == "content");

            var link = _rssItem.Elements().First(element => element.Name.LocalName == "link");
            _link = link.Value.Length == 0
                        ? link.Attributes().First(attribute => attribute.Name.LocalName == "href").Value
                        : link.Value;

            _creator = _rssItem.Elements().FirstOrDefault(element => element.Name.LocalName == "creator")
                          ?? _rssItem.Elements().First(element => element.Name.LocalName == "author")
                                 .Elements().First(element => element.Name.LocalName == "name");

            var dateElement = rssItem.Elements().FirstOrDefault(element => element.Name.LocalName == "pubDate")
                              ?? _rssItem.Elements().FirstOrDefault(element => element.Name.LocalName == "date")
                              ?? _rssItem.Elements().FirstOrDefault(element => element.Name.LocalName == "published");
            _date = dateElement != null ? GetDateTime(dateElement) : DateTime.Now;

            _title = _creator.Value + ": " + _originalTitle.Value;
        }

        private static DateTime GetDateTime(XElement element)
        {
            DateTime result;
            if (!DateTime.TryParse(element.Value, out result))
            {
                result = Convert.ToDateTime(element.Value.Replace("PDT", "GMT")).AddHours(-7);
            }
            return result;
        }

        public DateTime Date
        {
            get { return _date; }
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
            get { return _link; }
        }

        public XElement ToXml()
        {
            var element = new XElement("item",
                new XElement("title", _originalTitle.Value),
                new XElement("creator", _creator.Value),
                new XElement("description", _description.Nodes()),
                new XElement("link", _link),
                new XElement("date", _date.ToString()));
            return element;
        }
    }
}