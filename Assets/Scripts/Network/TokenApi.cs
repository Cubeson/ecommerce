using Network;
using Newtonsoft.Json;
using Shared.DTO;
using UnityEngine;
using UnityEngine.Networking;
using static Network.NetworkUtility;

namespace Network
{
    public static class TokenApi
    {
        public static UnityWebRequest RefreshToken (TokenModelDTO tokenModel)
        {
            var json = JsonConvert.SerializeObject(tokenModel);
            UnityWebRequest req = new UnityWebRequest()
            {
                method = "POST",
                url = Url + "api/Token/Refresh",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = Constants.DEFAULT_TIMEOUT,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            return req;
        }
        public static UnityWebRequest RevokeToken(TokenModelDTO tokenModel)
        {
            UnityWebRequest req = new UnityWebRequest()
            {
                method = "POST",
                url = Url + "api/Token/Revoke",
                downloadHandler = new DownloadHandlerBuffer(),
                //uploadHandler = UHR(json),
                timeout = Constants.DEFAULT_TIMEOUT,
            };
            req.SetRequestHeader("Authorization", "Bearer " + tokenModel.AuthToken );
            req.SetRequestHeader("Content-Type", "application/json");
            return req;
        }
    }

}