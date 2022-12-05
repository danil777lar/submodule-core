using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;

namespace Larje.Core.Tools
{
    public static class ConnectionTest
    {
        public static async Task<bool> IsConnectionAvailable() 
        {
            string htmlText = await GetHtmlFromUri("https://google.com");
            if (string.IsNullOrEmpty(htmlText) || string.IsNullOrWhiteSpace(htmlText))
            {
                return false;
            }
            else if (!htmlText.Contains("schema.org/WebPage"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static async Task<string> GetHtmlFromUri(string resource)
        {
            UnityWebRequest request = UnityWebRequest.Get(resource);
            request.SendWebRequest();
            while (!request.isDone) 
            {
                await Task.Yield();
            }

            bool isError = request.result == UnityWebRequest.Result.ConnectionError;
            isError |= request.result == UnityWebRequest.Result.ProtocolError;
            if (isError)
            {
                return "";
            }
            else 
            {
                return request.downloadHandler.text;
            }
        }
    }
}
