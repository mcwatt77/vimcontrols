using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Navigator.Path.Jobs;

namespace Navigator.Path.KevinBacon
{
    public class KevinBaconSearch
    {
        private readonly XElement _baconElement;
        private readonly Dictionary<string, XElement> _existingActorLookup;
        private readonly Dictionary<string, XElement> _existingMovieLookup;

        public KevinBaconSearch(XElement baconElement)
        {
            _baconElement = baconElement;

            _existingActorLookup = baconElement
                .Elements("actor")
                .Where(element => element.Elements("link").Count() > 0
                                  || element.Attributes("name").First().Value == "Kevin Bacon")
                .ToDictionary(element => element.Attributes("name").First().Value);

            _existingMovieLookup = baconElement
                .Elements("movie")
                .ToDictionary(element => MovieKeyFromElement(element));
        }

        private static string MovieKeyFromElement(XElement element)
        {
            return element.Attributes("name").First().Value.ToLower() + "_"
                   + (element.Attribute("releaseDate") != null
                          ? element.Attributes("releaseDate").First().Value
                          : "");
        }

        public void UpdateForActor(string actor)
        {
            if (_existingActorLookup.ContainsKey(actor))
                return;

            //TODO: This won't work because it doesn't retain the link back to the original actor
            //It only works if they've worked with somebody already linked.

            XElement linkedActor = null;
            var actors = new List<string> {actor};
            var actorTiers = new List<List<XElement>>();
            while (linkedActor == null)
            {
                var movies = GetMoviesForActors(actors);
                var actorElements = GetActorsForMovies(movies).ToList();
                actorTiers.Add(actorElements);
                linkedActor = ActorLinkedToKevinBacon(actorElements);
                actors = actorElements
                    .Select(element => element.Attributes("name").First().Value)
                    .ToList();
            }

            var debug = 0;
            //now add movies and actors to _baconElement, then create links for linkedActor
        }

        private IEnumerable<XElement> GetMoviesForActors(IEnumerable<string> actors)
        {
            IEnumerable<XElement> allMovies = new XElement[] {};
            foreach (var actor in actors)
                allMovies = allMovies.Concat(GetMoviesForActor(actor));
            return allMovies;
        }

        private IEnumerable<XElement> GetMoviesForActor(string actor)
        {
            const string baseUrl = "http://api.freebase.com/api/service/mqlread?query=";
            var query = baseUrl + "{\"query\":{\"type\":\"/film/actor\",\"name\":\"" + actor + "\", \"film\":[{\"film\":[{\"name\":null,\"initial_release_date\":null}]}]}}";

            string result;
            try
            {
                result = PageCacher.GetCachedPage(query);
            }
            catch
            {
                return new XElement[] {};
            }

            var document = JsonToXmlConvert.ConvertToXml(result);
            return document
                .Descendants("film")
                .Where(element => element.Ancestors().Skip(1).First().Name.LocalName == "film")
                .Select(filmElement => filmElement.Elements("group").First())
                .Select(element => new
                                       {
                                           Name = element.Elements("name").First().Value,
                                           ReleaseDate =
                                       FormatDateString(element.Elements("initial_release_date").First().Value)
                                       })
                .SelectMany(a => GetMovieElements(actor, a.Name, a.ReleaseDate));
        }

        private IEnumerable<XElement> GetMovieElements(string actor, string name, string releaseDate)
        {
            var movies = new List<XElement>();
            if (_existingMovieLookup.ContainsKey(name.ToLower() + "_" + releaseDate))
            {
                movies.Add(_existingMovieLookup[name.ToLower() + "_" + releaseDate]);
            }
            else
            {
                var newMovies = BuildNewMovieElements(name).ToList();
                foreach (var movie in newMovies)
                {
                    if (!_existingMovieLookup.ContainsKey(MovieKeyFromElement(movie)))
                    {
                        _baconElement.Add(movie);
                        if (movie.Elements("actor").Any(element => element.Attributes("name").First().Value == actor))
                            movies.Add(movie);
                    }
                }
            }
            return movies;
        }

        private static IEnumerable<XElement> BuildNewMovieElements(string name)
        {
            const string baseUrl = "http://api.freebase.com/api/service/mqlread?query=";
            var query = baseUrl
                               + "{\"query\":{\"type\":\"/film/film\",\"name\":\""
                               + name
                               + "\", \"initial_release_date\":null"
                               + ", \"starring\":[{\"actor\":null}]}}";

            string result;
            try
            {
                result = PageCacher.GetCachedPage(query);
            }
            catch (Exception e)
            {
                return new XElement[] {};
            }

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

            return newMovies
                .Select(a => new XElement("movie",
                                          new XAttribute("name", a.Name),
                                          a.ReleaseDate == null ? null : new XAttribute("releaseDate", a.ReleaseDate),
                                          a
                                              .Element
                                              .Descendants("actor")
                                              .Select(
                                              element => new XElement("actor", new XAttribute("name", element.Value)))))
                .ToArray();
        }


        private static string FormatDateString(string source)
        {
            if (source.Length == 0) return null;
            if (source == "null") return null;
            if (source.Length == 4) return source;

            var dateTime = Convert.ToDateTime(source);
            return dateTime.ToShortDateString();
        }

        private XElement ActorLinkedToKevinBacon(IEnumerable<XElement> actorElements)
        {
            var foundActor = actorElements
                .Select(element => new {Name = element.Attributes("name").First().Value, Element = element})
                .Where(a => _existingActorLookup.ContainsKey(a.Name))
                .Select(a => new {Count = _existingActorLookup[a.Name].Elements("link").Count(), a.Element})
                .OrderBy(a => a.Count)
                .Select(a => a.Element)
                .FirstOrDefault();

            return foundActor;
        }

        private static IEnumerable<XElement> GetActorsForMovies(IEnumerable<XElement> movies)
        {
            return movies.SelectMany(movie => movie.Elements("actor"));
        }
    }
}