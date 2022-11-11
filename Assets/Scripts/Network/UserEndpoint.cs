using UnityEngine.Networking;
using static UnityEngine.Networking.UnityWebRequest;
using Newtonsoft.Json;
using Shared.DTO;
using System.Text;
namespace Network
{
    public static class UserEndpoint
    {
        
        /// <param name="str"></param>
        /// <returns>An UploadHanlderRaw with UTF8 encoding</returns>
        private static UploadHandler UHR(string str)
        {
            return new UploadHandlerRaw(Encoding.UTF8.GetBytes(str));
        }
        /// <summary>
        /// Creates an UnityWebRequest that requests creating a new user
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns>UnityWebRequest ready to call SendWebRequest on</returns>
        public static UnityWebRequest CreateUser(UserCreateDTOUnity userDTO)
        {
            var json = JsonConvert.SerializeObject(userDTO);
            var req = new UnityWebRequest()
            {
                method = "POST",
                url = ServerUrl.Url + "api/User/Create",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = 8,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            return req;
        }

        public static UnityWebRequest RequestResetPasswordCode(RequestResetPasswordUnity requestReset)
        {
            var json = JsonConvert.SerializeObject(requestReset);
            UnityWebRequest req = new UnityWebRequest()
            {
                method = "POST",
                url = ServerUrl.Url + "api/User/RequestResetPasswordCode",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
            };
            req.SetRequestHeader("Content-Type", "application/json");
            return req;
        }
        public static UnityWebRequest ResetPassword(ResetPasswordCredentialsUnity resetCredentials)
        {
            var json = JsonConvert.SerializeObject(resetCredentials);
            UnityWebRequest req = new UnityWebRequest()
            {
                method = "POST",
                url = ServerUrl.Url + "api/User/ResetPassword",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = 8,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            return req;
        }
    }
}
