using UnityEngine.Networking;
using static Network.NetworkUtility;
using static Constants;
using Newtonsoft.Json;
using Shared.DTO;
using UnityEditor.Experimental.GraphView;

namespace Network
{
    public static class CartApi
    {
        public static UnityWebRequest AddItem(CartItemDTO cartItemDTO, TokenModelDTO tokenModel)
        {
            var json = JsonConvert.SerializeObject(cartItemDTO);
            var req = new UnityWebRequest()
            {
                method = "PATCH",
                url = $"{Url}api/Cart/AddItem",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = DEFAULT_TIMEOUT,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Authorization", "Bearer " + tokenModel.AuthToken);
            return req;
        }
        public static UnityWebRequest RemoveItem(CartItemDTO cartItemDTO, TokenModelDTO tokenModel)
        {
            var json = JsonConvert.SerializeObject(cartItemDTO);
            var req = new UnityWebRequest()
            {
                method = "PATCH",
                url = $"{Url}api/Cart/RemoveItem",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = DEFAULT_TIMEOUT,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Authorization", "Bearer " + tokenModel.AuthToken);
            return req;
        }

        public static UnityWebRequest SaveCart(CartItemDTO[] cartDTO, TokenModelDTO tokenModel)
        {
            var json = JsonConvert.SerializeObject(cartDTO);
            var req = new UnityWebRequest()
            {
                method = "POST",
                url = $"{Url}api/Cart/SaveCart",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = DEFAULT_TIMEOUT,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Authorization", "Bearer " + tokenModel.AuthToken);
            return req;
        }

        public static UnityWebRequest GetCart(TokenModelDTO tokenModel)
        {
            var req = UnityWebRequest.Get($"{Url}api/Cart/GetCart");
            req.SetRequestHeader("Authorization", "Bearer " + tokenModel.AuthToken);
            return req;
        }
    }
}
