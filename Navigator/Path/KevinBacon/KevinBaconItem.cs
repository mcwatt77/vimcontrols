using System.Linq;
using System.Xml.Linq;
using Navigator.UI.Attributes;

namespace Navigator.Path.KevinBacon
{
    public class KevinBaconItem : ISummaryString, IDescriptionString
    {
        private readonly string _actorName;
        private readonly string _description;

        public KevinBaconItem(XElement element)
        {
            _actorName = element.Attributes("name").First().Value + " (" + element.Elements("link").Count() + ")";
            _description = element.Attributes("name").First().Value;
            var links = element.Elements("link").Select(linkElement => " to " + linkElement.Attributes("actor").First().Value
                                                           + " in " + linkElement.Attributes("movie").First().Value);
            _description += "\r\n";
            foreach (var link in links)
            {
                _description += "\r\n" + link;
            }

            _description += "\r\n";
        }

        public string Summary
        {
            get { return _actorName; }
        }

        public string Description
        {
            get { return _description; }
        }
    }
}