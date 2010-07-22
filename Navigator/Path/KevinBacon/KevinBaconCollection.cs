using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Navigator.Containers;
using Navigator.UI.Attributes;
using VIControls.Commands.Interfaces;

namespace Navigator.Path.KevinBacon
{
    public class KevinBaconCollection : ISummaryString, IModelChildren, IIsSearchable
    {
        private readonly IContainer _container;
        private readonly XDocument _bacon;

        public KevinBaconCollection(IContainer container)
        {
            _container = container;

            var fileInfo = new FileInfo("kevin_bacon.xml");

            if (fileInfo.Exists)
            {
                _bacon = XDocument.Load(fileInfo.FullName);
                return;
            }

            _bacon = new XDocument(
                new XElement("bacon",
                             new XElement("actor",
                                          new XAttribute("name", "Kevin Bacon")))
                );

            _bacon.Save(fileInfo.FullName);
        }

        public string Summary
        {
            get { return "The Kevin Bacon Game!"; }
        }

        public IEnumerable<object> Children
        {
            get
            {
                return _bacon
                    .Elements("bacon").First()
                    .Elements("actor")
                    .Select(element => (object) _container.Get<KevinBaconItem>(element));
            }
        }

        public void CommitSearch(string searchText)
        {
        }
    }
}