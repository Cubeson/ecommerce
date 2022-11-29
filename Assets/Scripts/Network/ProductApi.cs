using UnityEngine.Networking;
using static Network.NetworkUtility;
using static Constants;
namespace Network
{
    public static class ProductApi {

        public static UnityWebRequest GetProduct(int id)
        {
            var req = UnityWebRequest.Get($"{Url}api/Product/GetProduct?id={id}");
            req.timeout = DEFAULT_TIMEOUT;
            return req;
        }
        public static UnityWebRequest GetProducts(int offset, int count,string category)
        {
            var req = UnityWebRequest.Get($"{Url}api/Product/GetProducts?offset={offset}&count={count}&category={category}");
            req.timeout = DEFAULT_TIMEOUT;
            return req;
        }
        public static UnityWebRequest GetThumbnail(int id)
        {
            var req = UnityWebRequestTexture.GetTexture($"{Url}api/Product/GetThumbnail?id={id}");
            //req.timeout = DEFAULT_TIMEOUT;
            return req;
        }
        public static UnityWebRequest GetPictures(int id)
        {
            var req = UnityWebRequest.Get($"{Url}api/Product/GetThumbnail?id={id}");
            //req.timeout = DEFAULT_TIMEOUT;
            return req;
        }
        public static UnityWebRequest GetCategories()
        {
            var req = UnityWebRequest.Get($"{Url}api/Product/GetCategories");
            return req;
        }
    }
}
