using System.Collections.Generic;
using System.Linq;
using Navigator.Containers;
using Navigator.UI.Attributes;

namespace Navigator.Path.Agile
{
    public class AgileLinks : ISummaryString, IModelChildren
    {
        private readonly IContainer _container;

        public AgileLinks(IContainer container)
        {
            _container = container;
        }

        private IEnumerable<object> _children;

        public string Summary
        {
            get { return "Agile Links"; }
        }

        public IEnumerable<object> Children
        {
            get
            {
                if (_children == null)
                {
                    var downloader = _container.Get<AgileDownloader>();
                    const string baseUrl = "http://agile.southlaw.com/TargetProcessWorkQueues/Menu/";
                    var document = downloader.Download(baseUrl + "Index.mvc");

                    _children = document
                        .Descendants("a")
                        .Select(a => _container.Get<AgileLink>(baseUrl, a))
                        .ToArray();
                }

                return _children;
            }
        }
    }
}