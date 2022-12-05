using System.IO;
using System.Net;

namespace Larje.Core.Tools
{
    public static class ConnectionTest
    {
        public static bool IsConnectionAvailable() 
        {
            string HtmlText = GetHtmlFromUri("https://google.com");
            if (string.IsNullOrEmpty(HtmlText) || string.IsNullOrWhiteSpace(HtmlText))
            {
                return false;
            }
            else if (!HtmlText.Contains("schema.org/WebPage"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static string GetHtmlFromUri(string resource)
        {
            string html = string.Empty;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
            try
            {
                using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
                {
                    bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                    if (isSuccess)
                    {
                        using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                        {
                            char[] cs = new char[80];
                            reader.Read(cs, 0, cs.Length);
                            foreach (char ch in cs)
                            {
                                html += ch;
                            }
                        }
                    }
                }
            }
            catch
            {
                return "";
            }
            return html;
        }
    }
}
