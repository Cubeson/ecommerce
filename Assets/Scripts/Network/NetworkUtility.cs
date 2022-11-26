using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Network
{
    public class AcceptAllCertificates : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            return true;
        }
    }

    

    public static class NetworkUtility
    {

        /// <summary>
        /// Returns the Server Url with a backslash at the end
        /// </summary>
        //public static string Url { get; private set; } = "https://localhost:443/";

        public static string Url { get {
#if !UNITY_WEBGL || UNITY_EDITOR
                return "https://localhost:443/";

#elif UNITY_WEBGL
                return Application.absoluteURL;
#endif
            }
        }

        /// <param name="str"></param>
        /// <returns>An UploadHandlerRaw with UTF8 encoding</returns>
        public static UploadHandler UHR(string str)
        {
            return new UploadHandlerRaw(Encoding.UTF8.GetBytes(str));
        }

    }
}

