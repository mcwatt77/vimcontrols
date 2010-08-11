using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Navigator.UI.Attributes;

namespace Navigator.Path.Agile
{
    public class AgileLink : ISummaryString, IModelChildren
    {
        private readonly AgileDownloader _downloader;
        private readonly string _title;
        private readonly string _href;

        public AgileLink(string baseUrl, XElement a, AgileDownloader downloader)
        {
            _downloader = downloader;
            _title = a.Value;
            _href = baseUrl + a.Attributes("href").First().Value;
        }

        public string Summary
        {
            get { return _title; }
        }

        public IEnumerable<object> Children
        {
            get
            {
                var document = _downloader.Download(_href);
                var trElements = document.Descendants("tr");
                return trElements
                    .Select(tr => new AgileRow(tr))
                    .ToArray();
            }
        }
    }

    public class AgileRow : IHasXml
    {
        private readonly XElement _tr;

        public AgileRow(XElement tr)
        {
            _tr = tr;
        }

        public XElement Xml
        {
            get { return _tr; }
        }
    }
}