using Network;
using Newtonsoft.Json;
using Shared.DTO;
using UnityEngine.Networking;
using static Network.NetworkUtility;

namespace Assets.Scripts.Network
{
    public static class TokenApi
    {
        public static UnityWebRequest RefreshToken (TokenModelUnity tokenModel)
        {
            var json = JsonConvert.SerializeObject(tokenModel);
            UnityWebRequest req = new UnityWebRequest()
            {
                method = "POST",
                url = Url + "api/User/RequestResetPasswordCode",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = Constants.Timeout,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            return req;
        }
    }
}
