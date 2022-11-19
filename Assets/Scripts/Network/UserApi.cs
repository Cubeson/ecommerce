using UnityEngine.Networking;
using Newtonsoft.Json;
using Shared.DTO;
using static Network.NetworkUtility;
namespace Network
{
    public static class UserApi
    {
        public static UnityWebRequest LoginUser(UserLoginDTOUnity userDTO)
        {
            var json = JsonConvert.SerializeObject(userDTO);
            var req = new UnityWebRequest()
            {
                method = "POST",
                url = Url + "api/User/Login",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = Constants.Timeout,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            return req;
        }
        public static UnityWebRequest CreateUser(UserCreateDTOUnity userDTO)
        {
            var json = JsonConvert.SerializeObject(userDTO);
            var req = new UnityWebRequest()
            {
                method = "POST",
                url = Url + "api/User/Create",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = Constants.Timeout,
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
                url = Url + "api/User/RequestResetPasswordCode",
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
                url = Url + "api/User/ResetPassword",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = Constants.Timeout,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            return req;
        }
        public static UnityWebRequest RevokeAllSessions(UserLoginDTOUnity userCredentials)
        {
            var json = JsonConvert.SerializeObject(userCredentials);
            UnityWebRequest req = new UnityWebRequest()
            {
                method = "POST",
                url = Url + "api/User/RevokeAllSessions",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = Constants.Timeout,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            return req;
        }
    }
}
