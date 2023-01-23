using UnityEngine.Networking;
using Shared.DTO;
using static Network.NetworkUtility;
using Newtonsoft.Json;

namespace Network
{
    public static class UserApi
    {
        public static UnityWebRequest LoginUser(UserLoginDTO userDTO)
        {
            var json = JsonConvert.SerializeObject(userDTO);
            var req = new UnityWebRequest()
            {
                method = "POST",
                url = Url + "api/User/Login",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = Constants.DEFAULT_TIMEOUT,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            req.certificateHandler = new AcceptAllCertificates();
            return req;
        }
        public static UnityWebRequest CreateUser(UserCreateDTO userDTO)
        {
            var json = JsonConvert.SerializeObject(userDTO);
            var req = new UnityWebRequest()
            {
                method = "POST",
                url = Url + "api/User/Create",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = Constants.DEFAULT_TIMEOUT,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            req.certificateHandler = new AcceptAllCertificates();
            return req;
        }

        public static UnityWebRequest RequestResetPasswordCode(RequestResetPasswordDTO requestReset)
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
            req.certificateHandler = new AcceptAllCertificates();
            return req;
        }
        public static UnityWebRequest ResetPassword(ResetPasswordCredentialsDTO resetCredentials)
        {
            var json = JsonConvert.SerializeObject(resetCredentials);
            UnityWebRequest req = new UnityWebRequest()
            {
                method = "POST",
                url = Url + "api/User/ResetPassword",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = Constants.DEFAULT_TIMEOUT,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            req.certificateHandler = new AcceptAllCertificates();
            return req;
        }
        public static UnityWebRequest RevokeAllSessions(UserLoginDTO userCredentials)
        {
            var json = JsonConvert.SerializeObject(userCredentials);
            UnityWebRequest req = new UnityWebRequest()
            {
                method = "POST",
                url = Url + "api/User/RevokeAllSessions",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = Constants.DEFAULT_TIMEOUT,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            req.certificateHandler = new AcceptAllCertificates();
            return req;
        }
    }
}
