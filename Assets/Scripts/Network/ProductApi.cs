using UnityEngine.Networking;
using static Network.NetworkUtility;
using static Constants;
using Shared.SortOrderDB;
using Assets.Scripts.Extensions;

namespace Network
{
    public static class ProductApi {

        public static UnityWebRequest GetProduct(int id)
        {
            var req = UnityWebRequest.Get($"{Url}api/Product/GetProduct?id={id}");
            req.timeout = DEFAULT_TIMEOUT;
            return req;
        }
        public static UnityWebRequest GetProducts(int offset, int count,string category, SortOrderDB sortOrder = SortOrderDB.DateModified_Desc, string nameFilter = "", decimal minPrice = 0, decimal maxPrice = 0)
        {
            string qNameFilter = "";
            if (!nameFilter.IsNullOrEmpty())
            {
                qNameFilter = $"&titleContains={nameFilter}";
            }
            string qMaxPrice = "";
            if(maxPrice > 0)
            {
                qMaxPrice = $"&maxPrice={maxPrice}";
            }
            string qMinPrice = "";
            if(minPrice > 0)
            {
                qMinPrice = $"&minPrice={minPrice}";
            }

            var req = UnityWebRequest.Get($"{Url}api/Product/GetProducts?offset={offset}&count={count}&category={category}&sortOrder={sortOrder}{qNameFilter}{qMinPrice}{qMaxPrice}");
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
            var req = UnityWebRequest.Get($"{Url}api/Product/GetPictures?id={id}");
            //req.timeout = DEFAULT_TIMEOUT;
            return req;
        }
        public static UnityWebRequest GetCategories()
        {
            var req = UnityWebRequest.Get($"{Url}api/Product/GetCategories");
            return req;
        }
        public static UnityWebRequest GetModel(int id)
        {
            var req = UnityWebRequest.Get($"{Url}api/Product/GetModel?id={id}");
            return req;
        }
    }
}
