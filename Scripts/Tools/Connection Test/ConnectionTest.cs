using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine.Networking;
using UnityEngine;

namespace Larje.Core.Tools
{
    public static class ConnectionTest
    {
        public static async Task<bool> IsConnectionAvailable(float timeOutSize) 
        {
#if !UNITY_WEBGL
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                return false;
            }

            Ping ping = new Ping("8.8.8.8");
            float pingStartTime = Time.time;

            while (!ping.isDone)
            {
                await Task.Delay(5);

                if (Time.time - pingStartTime >= timeOutSize)
                {
                    return false;
                }
            }
#endif
            return true;
        }
    }
}
