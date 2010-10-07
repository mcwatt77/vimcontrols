using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace Navigator.Path.Wow
{
    public class AhDownloader
    {
        public void Download()
        {
            var webclient = new CookieClient();
/*//            webclient.DownloadFile("https://us.battle.net/login/en/login.xml?ref=http://www.wowarmory.com/index.xml&app=armory&cr=true", @"d:\login.xml");
            webclient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            var postData = "accountName=vorbisus@yahoo.com&password=n3v3rgu3ss&persistLogin=on";
//            var postData = "auction_sk=b963cf64-5625-452d-865f-fa16902f57ce&JSESSIONID=40E9639F690D4406A57604DF1680C51B.blade22_08";
//            var postData = "";
            var bytes = Encoding.ASCII.GetBytes(postData);
            bytes = webclient.UploadData("https://us.battle.net/login/en/login.xml?ref=http://www.wowarmory.com/index.xml&app=armory&cr=true", "POST", bytes);
//            bytes = webclient.UploadData("http://wowarmory.com/auctionhouse/index.xml", "POST", bytes);
            var result = Encoding.ASCII.GetString(bytes);
            var writer = new StreamWriter(@"d:\login.xml");
            writer.Write(result);
            writer.Flush();
            writer.Close();*/

            var postData = "accountName=vorbisus@yahoo.com&password=n3v3rgu3ss&persistLogin=on";
            webclient.DownloadFile("https://us.battle.net/login/en/login.xml?ref=http://www.wowarmory.com/index.xml&app=armory&cr=true", "login.xml");
            var bytes = Encoding.ASCII.GetBytes(postData);
            bytes = webclient.UploadData("https://us.battle.net/login/en/login.xml?ref=http://www.wowarmory.com/index.xml&app=armory&cr=true", "POST", bytes);
            var result = Encoding.ASCII.GetString(bytes);
            var writer = new StreamWriter(@"d:\test.xml");
            writer.Write(result);
            writer.Flush();
            writer.Close();

            //https://us.battle.net/login/en/login.xml?ref=http://www.wowarmory.com/index.xml&app=armory&cr=true

//            webclient.DownloadFile("http://www.wowarmory.com/auctionhouse/summary/?rhtml=true&cn=Gnoghway&r=Eldre'Thalas&f=0&sk=2e295856-b9e2-4fad-a293-eba701deecc1", "test.xml");

                //https://us.battle.net/login/en/login.xml?ref=http://www.wowarmory.com/index.xml&app=armory&cr=true

        }
    }

    public class CookieClient : WebClient
    {
        private CookieContainer m_container = new CookieContainer();

        protected override WebRequest GetWebRequest(System.Uri address)
        {
            WebRequest request = base.GetWebRequest(address);
            if (request is HttpWebRequest)
            {
                (request as HttpWebRequest).CookieContainer = m_container;
            }
            return request;
        }

        public static string UploadFileEx(string uploadfile, string url,
    string fileFormName, string contenttype, NameValueCollection querystring,
    CookieContainer cookies)
        {
            if (string.IsNullOrEmpty(fileFormName))
            {
                fileFormName = "file";
            }

            if (string.IsNullOrEmpty(contenttype))
            {
                contenttype = "application/octet-stream";
            }


            string postdata = "?";
            if (querystring != null)
            {
                foreach (string key in querystring.Keys)
                {
                    postdata += key + "=" + querystring.Get(key) + "&";
                }
            }
            var uri = new Uri(url + postdata);


            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            var webrequest = (HttpWebRequest)WebRequest.Create(uri);
            webrequest.CookieContainer = cookies;
            webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webrequest.Method = "POST";


            // Build up the post message header

            var sb = new StringBuilder();
            sb.Append("--");
            sb.Append(boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append(fileFormName);
            sb.Append("\"; filename=\"");
            
            sb.Append(new FileInfo(uploadfile).Name);
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append(contenttype);
            sb.Append("\r\n");
            sb.Append("\r\n");

            string postHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);

            // Build the trailing boundary string as a byte array

            // ensuring the boundary appears on a line by itself

            byte[] boundaryBytes =
                   Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            var fileStream = new FileStream(uploadfile,
                                        FileMode.Open, FileAccess.Read);
            long length = postHeaderBytes.Length + fileStream.Length +
                                                   boundaryBytes.Length;
            webrequest.ContentLength = length;

            Stream requestStream = webrequest.GetRequestStream();

            // Write out our post header

            requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

            // Write out the file contents

            var buffer = new Byte[checked((uint)Math.Min(4096,
                                     (int)fileStream.Length))];
            int bytesRead;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                requestStream.Write(buffer, 0, bytesRead);

            // Write out the trailing boundary

            requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
            WebResponse responce = webrequest.GetResponse();
            Stream s = responce.GetResponseStream();
            var sr = new StreamReader(s);

            return sr.ReadToEnd();
        }
    }

}