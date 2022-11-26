using Network;
using Shared.DTO;
using UnityEngine;
using UnityEngine.Networking;
using static Network.NetworkUtility;

namespace Assets.Scripts.Network
{
    public static class TokenApi
    {
        public static UnityWebRequest RefreshToken (TokenModelDTOUnity tokenModel)
        {
            var json = JsonUtility.ToJson(tokenModel);
            UnityWebRequest req = new UnityWebRequest()
            {
                method = "POST",
                url = Url + "api/Token/Refresh",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = Constants.Timeout,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            req.certificateHandler = new AcceptAllCertificates();
            return req;
        }
        public static UnityWebRequest RevokeToken(TokenModelDTOUnity tokenModel)
        {
            //var json = JsonConvert.SerializeObject(tokenModel);
            UnityWebRequest req = new UnityWebRequest()
            {
                method = "POST",
                url = Url + "api/Token/Revoke",
                downloadHandler = new DownloadHandlerBuffer(),
                //uploadHandler = UHR(json),
                timeout = Constants.Timeout,
            };
            req.SetRequestHeader("Authorization", "Bearer " + tokenModel.AuthToken );
            req.certificateHandler = new AcceptAllCertificates();
            //req.SetRequestHeader("Content-Type", "application/json");
            return req;
        }
    }

}