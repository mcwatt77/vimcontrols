using System.Net;
using System.Text;
using System.Xml.Linq;
using Navigator.Path.Jobs;

namespace Navigator.Path.Agile
{
    public class AgileDownloader
    {
        private readonly string _password;
        private readonly WebClient _webClient;

        public AgileDownloader()
        {
            _webClient = new WebClient();

            if (_password == null)
//                _password = PasswordPrompt.RetrievePassword();
                _password = "P1d3rm0nk3y";

            _webClient.Credentials = new NetworkCredential("mikev", _password, "southlawad");
        }

        public XDocument Download(string url)
        {
            var data = _webClient.DownloadData(url);
            var page = Encoding.ASCII.GetString(data);
            return XmlConverter.ConvertToXml(page);
        }
    }
}