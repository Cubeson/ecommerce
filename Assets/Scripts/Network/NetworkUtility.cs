using System.Text;
using UnityEngine.Networking;

namespace Network
{
    public static class NetworkUtility
    {
        /// <summary>
        /// Returns the Server Url with a backslash at the end
        /// </summary>
        public static string Url { get; private set; } = "https://localhost:6900/";

        /// <param name="str"></param>
        /// <returns>An UploadHandlerRaw with UTF8 encoding</returns>
        public static UploadHandler UHR(string str)
        {
            return new UploadHandlerRaw(Encoding.UTF8.GetBytes(str));
        }

    }
}

