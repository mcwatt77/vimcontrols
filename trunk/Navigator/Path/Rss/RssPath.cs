using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;
using Navigator.UI.Attributes;

namespace Navigator.Path.Rss
{
    public class RssPath : ISummaryString, IModelChildren
    {
        private readonly string _url;
        private readonly IEnumerable<object> _children;
        private readonly XElement[] _items;

        public RssPath(string url)
        {
            _url = url;
            var webClient = new WebClient();
            var buffer = webClient.DownloadData(url);
            var text = Encoding.ASCII.GetString(buffer);
            var doc = XDocument.Parse(text);

            var rdfElement = doc.Elements().First();
            _items = rdfElement.Descendants().Where(element => element.Name.LocalName == "item").ToArray();

            _children = _items.Select(element => (object)new RssItem(this, element)).ToArray();
        }

        public IEnumerable<object> Children
        {
            get { return _children; }
        }

        public string Summary
        {
            get { return "Rss feed for: " + _url; }
        }
    }
}