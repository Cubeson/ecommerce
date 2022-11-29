using UnityEngine.Networking;
using static Network.NetworkUtility;
using static Constants;
using Newtonsoft.Json;
using Shared.DTO;

namespace Network
{
    public static class CartApi
    {
        public static UnityWebRequest AddItem(CartItemDTO cartItemDTO)
        {
            var json = JsonConvert.SerializeObject(cartItemDTO);
            UnityWebRequest req = new UnityWebRequest()
            {
                method = "POST",
                url = $"{Url}api/Cart/AddItem",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = DEFAULT_TIMEOUT,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            return req;
        }

        public static UnityWebRequest SaveCart(CartDTO cartDTO)
        {
            var json = JsonConvert.SerializeObject(cartDTO);
            UnityWebRequest req = new UnityWebRequest()
            {
                method = "POST",
                url = $"{Url}api/Cart/SaveCart",
                downloadHandler = new DownloadHandlerBuffer(),
                uploadHandler = UHR(json),
                timeout = DEFAULT_TIMEOUT,
            };
            req.SetRequestHeader("Content-Type", "application/json");
            return req;
        }
    }
}
