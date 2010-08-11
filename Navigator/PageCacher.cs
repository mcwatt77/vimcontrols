using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace Navigator
{
    public static class PageCacher
    {
        public static string GetCachedPage(string url, params string[] cookies)
        {
            var referenceFile = new FileInfo("url_reference.xml");
            if (!referenceFile.Exists)
                new XDocument(new XElement("references")).Save(referenceFile.FullName);
            var references = XDocument.Load(referenceFile.FullName);

            var replace = url.Replace("\"", "%22");

            var urlElement = references
                .Descendants("url")
                .SingleOrDefault(element => element.Attributes("href").First().Value == replace);

            string actualFileName;

            if (urlElement == null)
            {
                actualFileName = Guid.NewGuid().ToString();
                urlElement = new XElement("url",
                    new XAttribute("href", replace),
                    new XAttribute("file", actualFileName));

                references.Elements("references").First().Add(urlElement);

                var webClient = new WebClient();
                webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                if (cookies != null)
                    webClient.Headers.Add("Cookie", cookies.Aggregate("", (s, a) => s + a));
                webClient.DownloadFile(url, actualFileName);

                references.Save(referenceFile.FullName);
            }
            else actualFileName = urlElement.Attributes("file").First().Value;

            var file = new FileInfo(actualFileName);
            return file.OpenText().ReadToEnd();
        }
    }
}