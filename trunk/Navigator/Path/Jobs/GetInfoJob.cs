using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Navigator.Containers;

namespace Navigator.Path.Jobs
{
    public class GetInfoJob : Job
    {
/*On the front page:
A long list of names.  I should be able to toggle between names and images (with names)
I should be able to mark a favorite.
I should also be able to set filters and sorting.
I should also be able to up rank, down rank, and set rank
I should be able to navigate to the subject, and see all related media and data.*/
        public GetInfoJob(IContainer container) : base(container)
        {
        }

        public override string Summary
        {
            get { return "Get Info"; }
        }

        public override void Execute()
        {
            var dictionary = new Dictionary<string, Person>();

            var fileInfo = new FileInfo("name_info.xml");
            if (!fileInfo.Exists)
            {
                new XDocument(
                    new XElement("names",
                                 new XElement("sites")))
                    .Save(fileInfo.FullName);
            }
            var document = XDocument.Load(fileInfo.FullName);
            var sites = document
                .Elements("names").First()
                .Elements("sites").First()
                .Elements("site")
                .Select(element => element.Value)
                .ToArray();

/*            if (!sites.Contains("kink"))
                ExecuteKink(dictionary);*/
            if (!sites.Contains("pornjax"))
                ExecutePornJax(dictionary);

            ExecuteFreeOnes();
        }

        private void ExecuteKink(IDictionary<string, Person> dictionary)
        {
            const string baseUrl = "http://www.kink.com/k/models.jsp";
            var result = GetCachedPage(baseUrl, "ageverified=1;");

            var document = XmlConverter.ConvertToXml(result);

            var links = document
                .Descendants("table")
                .Where(element => element.Attribute("id") != null)
                .First(element => element.Attributes("id").Single().Value == "modelsTable")
                .Descendants("a")
                .Select(element => element.Attributes("href").Single().Value)
                .ToArray();

            foreach (var link in links)
            {
                result = GetCachedPage("http://www.kink.com/k/" + link);

                document = XmlConverter.ConvertToXml(result);

                var name = document
                    .Descendants("meta")
                    .Where(element => element.Attribute("name") != null)
                    .First(element => element.Attributes("name").Single().Value == "Keywords")
                    .Attributes("content").Single().Value;

                UpdateSummary(name);

                links = document
                    .Descendants("a")
                    .Where(element => element.Attribute("class") != null)
                    .Where(element => element.Attributes("class").Single().Value == "clickableArea")
                    .Select(element => element.Attributes("href").Single().Value)
                    .ToArray();

                var person = dictionary.ContainsKey(name) ? dictionary[name] : new Person();

                var personLink = person.PersonLinks.FirstOrDefault(pl => pl.Site == "pornjax");
                if (personLink == null)
                {
                    personLink = new PersonLink {Site = "pornjax"};
                    person.PersonLinks.Add(personLink);
                }

                var debug = 0;
            }
        }

        private void ExecutePornJax(IDictionary<string, Person> dictionary)
        {
            const string site = "http://www.pornjax.com";
            const string baseUrl = site + "/category/Pornstars";

            var result = GetCachedPage(baseUrl, "ageverified=1;");

            var document = XmlConverter.ConvertToXml(result);

            var links = document
                .Descendants("div")
                .Where(element => element.Attribute("id") != null)
                .First(element => element.Attributes("id").Single().Value == "pj_main")
                .Descendants("table").First()
                .Descendants("li")
                .Select(element => element.Elements("a").First())
                .Select(element => element.Attributes("href").Single().Value)
                .ToArray();

            foreach (var partialLink in links)
            {
                var link = site + partialLink + "/galleries";
                UpdateSummary(link);

                var name = partialLink.Replace('_', ' ');
                var person = dictionary.ContainsKey(name) ? dictionary[name] : new Person();

                var personLink = person.PersonLinks.FirstOrDefault(pl => pl.Site == "pornjax");
                if (personLink == null)
                {
                    personLink = new PersonLink {Site = "pornjax"};
                    person.PersonLinks.Add(personLink);
                }

                result = GetCachedPage(link, "ageverified=1;");

                document = XmlConverter.ConvertToXml(result);

                var debug = 0;
            }
        }

        private void ExecuteFreeOnes()
        {
            var fileInfo = new FileInfo("full_name_list.xml");
            var names = new List<string>();
            if (!fileInfo.Exists)
            {
                foreach (var c in Enumerable.Range(0, 26).Select(i => (char) ('a' + i)))
                    ProcessIndexPage(c, names);

                var streamWriter = new StreamWriter(fileInfo.FullName);

                foreach (var name in names)
                    streamWriter.WriteLine(name);

                streamWriter.Flush();
                streamWriter.Close();
            }
            else
            {
                names = Regex.Split(fileInfo
                                        .OpenText()
                                        .ReadToEnd(), "\r\n")
                    .ToList();
            }

            var links = new List<string>();
            foreach (var name in names)
            {
//                ProcessPerson(name, links);

                //should save this off to some sort of persisten store, then reset links
                links.Clear();
            }

            UpdateSummary("Get Info (Complete");
        }

// ReSharper disable PossibleNullReferenceException
        private void ProcessPerson(string name, List<string> links)
        {
            var url = "http://www.freeones.com/html/" + name[0] + "_links/" + name;

            Func<XDocument, IEnumerable<string>> fnGetLinks = doc =>
                doc.Descendants("td")
                .Where(element => element.Attribute("class") != null)
                .Where(element => element.Attribute("class").Value == "Link")
                .Select(element => element.Descendants("a").Single(a => a.Elements().Count() == 0).Attribute("href").Value);

            ProcessPageWithLinks(url, links, fnGetLinks);
        }

        private void ProcessIndexPage(char c, List<string> names)
        {
            var url = "http://www.freeones.com/html/" + c + "_links/";

            Func<XDocument, IEnumerable<string>> fnGetLinks = doc =>
                doc.Descendants("a")
                .Where(element => element.Attribute("onmouseover") != null)
                .Select(element => element.Attribute("href").Value);

            ProcessPageWithLinks(url, names, fnGetLinks);
        }

        private void ProcessPageWithLinks(string url, List<string> names, Func<XDocument, IEnumerable<string>> fnGetLinks)
        {
            var document = ProcessPage(url, names, fnGetLinks);

            var pages = document.Descendants("a")
                .Where(element => element.Parent.Name.LocalName == "span" && element.Parent.Attribute("class") != null)
                .Where(element => element.Parent.Attribute("class").Value.Trim() == "page_number")
                .Select(element => element.Attribute("href").Value)
                .Distinct()
                .ToArray();

            foreach (var page in pages)
                ProcessPage(url + page, names, fnGetLinks);
        }
// ReSharper restore PossibleNullReferenceException

        private XDocument ProcessPage(string url, List<string> names, Func<XDocument, IEnumerable<string>> fnGetLinks)
        {
            var result = GetCachedPage(url);
            var document = XmlConverter.ConvertToXml(result);

            names.AddRange(fnGetLinks(document));

            return document;
        }

        public string GetCachedPage(string url, params string[] cookies)
        {
            return PageCacher.GetCachedPage(url, cookies);
        }
    }

    public class Person
    {
        private readonly List<PersonLink> _personLinks = new List<PersonLink>();

        public string Name { get; set; }

        public List<PersonLink> PersonLinks
        {
            get
            {
                return _personLinks;
            }
        }
    }

    public class PersonLink
    {
        private readonly List<string> _links = new List<string>();

        public string Site { get; set; }

        public List<string> Links
        {
            get
            {
                return _links;
            }
        }
    }
}