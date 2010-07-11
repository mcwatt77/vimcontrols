using System;
using System.Collections.Generic;
using System.IO;
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

        public RssPath(string url)
        {
            _url = url;

            _children = new DeferredExecutionList<object>(() => GetChildren(CacheWebPath(url)).Cast<object>().ToArray());
        }

        private IEnumerable<RssItem> GetChildren(XContainer doc)
        {
            var rdfElement = doc.Elements().First();

            var items = rdfElement.Descendants().Where(element => element.Name.LocalName == "item").ToArray();
            if (items.Length == 0)
                items = rdfElement.Descendants().Where(element => element.Name.LocalName == "entry").ToArray();

            return items.Select(element => new RssItem(this, element)).ToArray();
        }

        private XDocument CacheWebPath(string url)
        {
            XDocument document;
            var file = new FileInfo(url.Replace("/", "%2F").Replace(":", "%3A"));
            if (!file.Exists)
            {
                var webClient = new WebClient();
                var buffer = webClient.DownloadData(url);
                var text = Encoding.ASCII.GetString(buffer);
                document = XDocument.Parse(text);

                document.Save(file.FullName);
            }
            else
            {
                document = XDocument.Load(file.FullName);

                if (DateTime.Now.Subtract(file.LastWriteTime).TotalMinutes > 10)
                {
                    var webClient = new WebClient();
                    var buffer = webClient.DownloadData(url);
                    var text = Encoding.ASCII.GetString(buffer);
                    var additionalDocument = XDocument.Parse(text);

                    var oldChildren = GetChildren(document).ToArray();
                    var newChildren = GetChildren(additionalDocument).ToArray();

                    var allChildren = oldChildren
                        .Concat(newChildren)
                        .GroupBy(child => child.Description)
                        .Select(g => g.Last())
                        .ToArray();

                    var element = new XElement("rss",
                        new XElement("channel",
                            allChildren.Select(child => child.ToXml())));

                    element.Save(file.FullName);

                    document = XDocument.Load(file.FullName);
                }
            }

            return document;
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