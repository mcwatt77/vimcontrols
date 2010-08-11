using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Navigator.Containers;
using Navigator.Path.Jobs;
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
            var kevinBaconSearch = new KevinBaconSearch(_bacon.Elements("bacon").First());
            kevinBaconSearch.UpdateForActor(searchText);

/*            var actorElement = UpdateActor(searchText);
            UpdateMovies(actorElement);
            var actorLinks = GetKevinBaconLinks();

            if (actorLinks != null)
                actorElement.Add(actorLinks);

            var fileInfo = new FileInfo("kevin_bacon.xml");
            _bacon.Save(fileInfo.FullName);*/
        }

        private IEnumerable<XElement> GetKevinBaconLinks()
        {
            var movies = _bacon.Elements("bacon").First()
                .Elements("movie");

            var actorLookup = _bacon.Elements("bacon").First()
                .Elements("actor")
                .Where(element => element.Elements("link").Count() > 0
                                  || element.Attributes("name").First().Value == "Kevin Bacon")
                .ToDictionary(element => element.Attributes("name").First().Value);

            var actorElements = movies
                .SelectMany(element => element.Elements("actor"));

            foreach (var actor in actorElements)
            {
                var actorName = actor.Attributes("name").First().Value;
                if (!actorLookup.ContainsKey(actorName)) continue;

                var linkElements = actorLookup[actorName].Elements("link");

                var newLink = new XElement("link",
                                           new XAttribute("movie", actor.Ancestors().First().Attributes("name").First().Value),
                                           new XAttribute("actor", actor.Attributes("name").First().Value));

                return new[] {newLink}.Concat(linkElements);
            }

            return null;
        }

        private void UpdateMovies(XContainer actorElement)
        {
            var movies = actorElement.Elements("movie");
            foreach (var movieElement in movies)
                UpdateMovie(movieElement);
        }

        private void UpdateMovie(XElement movieElement)
        {
            var movieName = movieElement.Attributes("name").First().Value;

            const string baseUrl = "http://api.freebase.com/api/service/mqlread?query=";
            var query = baseUrl
                               + "{\"query\":{\"type\":\"/film/film\",\"name\":\""
                               + movieName
                               + "\", \"initial_release_date\":null"
                               + ", \"starring\":[{\"actor\":null}]}}";

            var result = PageCacher.GetCachedPage(query);

            var document = JsonToXmlConvert.ConvertToXml(result);
            var newMovies = document
                .Descendants("starring")
                .Where(element => element.Descendants("actor").Any(actor => actor.Value.Length > 0))
                .Select(element => new
                                       {
                                           Name = element.Ancestors().First().Elements("name").First().Value,
                                           ReleaseDate = FormatDateString(element.Ancestors().First().Elements("initial_release_date").First().Value),
                                           Element = element
                                       })
                .ToArray();

            var existingMovies = _bacon
                .Elements("bacon").First()
                .Elements("movie")
                .Select(element => new
                                       {
                                           Name = element.Attributes("name").First().Value,
                                           InitialDate = element.Attribute("releaseDate") == null
                                                             ? null
                                                             : element.Attributes("releaseDate").First().Value
                                       })
                .Where(a => a.Name.Equals(movieName, StringComparison.InvariantCultureIgnoreCase))
                .ToArray();

            newMovies = newMovies
                .Where(
                a => !existingMovies.Any(existing => existing.InitialDate == a.ReleaseDate
                                                    || (existing.InitialDate == null && a.ReleaseDate == null)))
                .ToArray();

            var newMovieElements = newMovies
                .Select(a => new XElement("movie",
                                          new XAttribute("name", a.Name),
                                          a.ReleaseDate == null ? null : new XAttribute("releaseDate", a.ReleaseDate),
                                          a
                                              .Element
                                              .Descendants("actor")
                                              .Select(
                                              element => new XElement("actor", new XAttribute("name", element.Value)))))
                .ToArray();

            _bacon.Elements("bacon").First().Add(newMovieElements);
        }

        private static string FormatDateString(string source)
        {
            if (source.Length == 0) return null;
            if (source == "null") return null;
            if (source.Length == 4) return source;

            var dateTime = Convert.ToDateTime(source);
            return dateTime.ToShortDateString();
        }

        private XElement UpdateActor(string actor)
        {
            const string baseUrl = "http://api.freebase.com/api/service/mqlread?query=";
            var query = baseUrl + "{\"query\":{\"type\":\"/film/actor\",\"name\":\"" + actor + "\", \"film\":[{\"film\":[{\"name\":null,\"initial_release_date\":null}]}]}}";

            var result = PageCacher.GetCachedPage(query);

            var document = JsonToXmlConvert.ConvertToXml(result);
            var films = document
                .Descendants("film")
                .Where(element => element.Ancestors().Skip(1).First().Name.LocalName == "film")
                .Select(filmElement => filmElement.Elements("group").First())
                .Select(element => new
                                       {
                                           Name = element.Elements("name").First().Value,
                                           ReleaseDate = FormatDateString(element.Elements("initial_release_date").First().Value)
                                       })
                .ToArray();

            var baconElement = _bacon.Elements("bacon").First();
            var actorElement = baconElement
                .Elements("actor").FirstOrDefault(element => element.Attributes("name").First().Value == actor);

            if (actorElement == null)
            {
                actorElement = new XElement("actor",
                                            new XAttribute("name", actor));
                baconElement.Add(actorElement);
            }

            var existingMovies = actorElement
                .Elements("movie")
                .Select(element => element.Attributes("name").First().Value)
                .Distinct()
                .ToDictionary(movie => movie);

            films = films
                .Where(film => !existingMovies.ContainsKey(film.Name)).ToArray();

            foreach (var film in films)
            {
                var movie = new XElement("movie", new XAttribute("name", film.Name));
                actorElement.Add(movie);

                if (film.ReleaseDate != null)
                    movie.Add(new XAttribute("releaseDate", film.ReleaseDate));

            }

            return actorElement;
        }
    }
}