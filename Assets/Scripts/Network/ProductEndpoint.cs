using Network.DTO;
using static UnityEngine.Networking.UnityWebRequest;
using Debug = UnityEngine.Debug;
using Newtonsoft.Json;
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
        public static byte[] DownloadProduct(int id)
        {
            var op = Get(ServerUrl.Url + "Download/id/"+id).SendWebRequest();
            while (!op.isDone) { }
            return op.webRequest.downloadHandler.data;
        }
    }
}
