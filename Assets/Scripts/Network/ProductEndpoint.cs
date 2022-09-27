using Network.DTO;
using static UnityEngine.Networking.UnityWebRequest;
using Debug = UnityEngine.Debug;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Network
{
    internal static class ProductEndpoint
    {
        public static ProductDTO GetProductInfo(int id)
        {
            var op = Get(ServerUrl.Url + "Product/id/" + id).SendWebRequest();
            while (!op.isDone) { }
            string resp = op.webRequest.downloadHandler.text;
            ProductDTO productDTO;
            try
            {
                productDTO = JsonConvert.DeserializeObject<ProductDTO>(resp);
            }
            catch (JsonException)
            {
                return null;
            }
            return productDTO;
            
        }
        public static ProductDTO[] GetProducts(int limit)
        {
            var op = Get(ServerUrl.Url+ "Product/latest/"+limit).SendWebRequest();
            while (!op.isDone) { }
            string data = op.webRequest.downloadHandler.text;
            Debug.Log(data);
            var products = JsonConvert.DeserializeObject<ProductDTO[]>(data);
            return products;
        }
        public static byte[] DownloadProductModel(int id)
        {
            var op = Get(ServerUrl.Url + "Download/model/"+id).SendWebRequest();
            while (!op.isDone) { }
            return op.webRequest.downloadHandler.data;
        }
        public static byte[] DownloadProductThumbnail(int id)
        {
            var op = Get(ServerUrl.Url + "Download/thumbnail/" + id).SendWebRequest();
            while (!op.isDone) { }
            return op.webRequest.downloadHandler.data;
        }
    }
}
