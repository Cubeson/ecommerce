using UnityEngine.Networking;
using Shared.DTO;
using static Network.NetworkUtility;
using UnityEngine;

namespace Network
{
    public static class UserApi
    {
        public static UnityWebRequest LoginUser(UserLoginDTOUnity userDTO)
        {
            var json = JsonUtility.ToJson(userDTO);
            var req = new UnityWebRequest()
            {
                method = "POST",
                url = Url + "api/User/Login",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = Constants.Timeout,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            req.certificateHandler = new AcceptAllCertificates();
            return req;
        }
        public static UnityWebRequest CreateUser(UserCreateDTOUnity userDTO)
        {
            var json = JsonUtility.ToJson(userDTO);
            var req = new UnityWebRequest()
            {
                method = "POST",
                url = Url + "api/User/Create",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = Constants.Timeout,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            req.certificateHandler = new AcceptAllCertificates();
            return req;
        }

        public static UnityWebRequest RequestResetPasswordCode(RequestResetPasswordDTOUnity requestReset)
        {
            var json = JsonUtility.ToJson(requestReset);
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
        public static UnityWebRequest ResetPassword(ResetPasswordCredentialsDTOUnity resetCredentials)
        {
            var json = JsonUtility.ToJson(resetCredentials);
            UnityWebRequest req = new UnityWebRequest()
            {
                method = "POST",
                url = Url + "api/User/ResetPassword",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = Constants.Timeout,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            req.certificateHandler = new AcceptAllCertificates();
            return req;
        }
        public static UnityWebRequest RevokeAllSessions(UserLoginDTOUnity userCredentials)
        {
            var json = JsonUtility.ToJson(userCredentials);
            UnityWebRequest req = new UnityWebRequest()
            {
                method = "POST",
                url = Url + "api/User/RevokeAllSessions",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = Constants.Timeout,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            req.certificateHandler = new AcceptAllCertificates();
            return req;
        }
    }
}
