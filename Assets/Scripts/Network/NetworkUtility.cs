using System.Text;
using TempProject;
using UnityEngine.Networking;

namespace Network
{
    public class AcceptAllCertificates : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData)
        {
            Class1Unity x = new Class1Unity();
            return true;
        }
    }

    public static class NetworkUtility
    {
        /// <summary>
        /// Returns the Server Url with a backslash at the end
        /// </summary>
        public static string Url { get; private set; } = "https://localhost:443/";


        /// <param name="str"></param>
        /// <returns>An UploadHandlerRaw with UTF8 encoding</returns>
        public static UploadHandler UHR(string str)
        {
            return new UploadHandlerRaw(Encoding.UTF8.GetBytes(str));
        }

    }
}

