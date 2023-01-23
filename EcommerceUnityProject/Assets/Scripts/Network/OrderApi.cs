using Shared.DTO;
using UnityEngine.Networking;
using static Constants;
using static Network.NetworkUtility;
namespace Network
{
    public static class OrderApi
    {
        public static UnityWebRequest CreateOrder(TokenModelDTO tokenModel)
        {
            UnityWebRequest req = new UnityWebRequest
            {
                method = "POST",
                url = $"{Url}api/Order/CreateOrder",
                downloadHandler = new DownloadHandlerBuffer(),
                timeout = Constants.DEFAULT_TIMEOUT,
            };
            req.SetRequestHeader("Authorization", "Bearer " + tokenModel.AuthToken);
            req.SetRequestHeader("Content-Type", "application/json");
            return req;
        }
        public static UnityWebRequest CancelOrder(TokenModelDTO tokenModel, int orderId)
        {
            UnityWebRequest req = new UnityWebRequest
            {
                method = "DELETE",
                url = $"{Url}api/Order/CancelOrder/{orderId}",
                downloadHandler = new DownloadHandlerBuffer(),
                timeout = Constants.DEFAULT_TIMEOUT,
            };
            req.SetRequestHeader("Authorization", "Bearer " + tokenModel.AuthToken);
            //req.SetRequestHeader("Content-Type", "application/json");
            return req;
        }
        public static UnityWebRequest GetOrder(int orderId)
        {
            UnityWebRequest req = UnityWebRequest.Get($"{Url}api/Order/GetOrder/{orderId}");
            req.timeout= Constants.DEFAULT_TIMEOUT;
            return req;
        }
        public static UnityWebRequest GetOrderStatus(int orderId)
        {
            UnityWebRequest req = UnityWebRequest.Get($"{Url}api/Order/GetOrderStatus/{orderId}");
            req.timeout = Constants.DEFAULT_TIMEOUT;
            req.SetRequestHeader("Content-Type", "application/json");
            return req;
        }
    }

}
