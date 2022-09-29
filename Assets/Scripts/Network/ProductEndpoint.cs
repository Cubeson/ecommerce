using Network.DTO;
using static UnityEngine.Networking.UnityWebRequest;
using Debug = UnityEngine.Debug;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.IO;

namespace Network
{
    internal static class ProductEndpoint
    {
        public static void Temp()
        {
            var formData = new List<IMultipartFormSection>();
            formData.Add(new MultipartFormDataSection("name","Unity"));
            formData.Add(new MultipartFormDataSection("description","UnityDesc"));
            formData.Add(new MultipartFormDataSection("price","10"));

            var fModel = File.ReadAllBytes("Resources/model.fbx");
            var fThumbnail = File.ReadAllBytes("Resources/thumbnail.png");
            var fArchive = File.ReadAllBytes("Resources/archive.zip");
            formData.Add(new MultipartFormFileSection("fileModel", fModel ,"model.fbx", "application/octet-stream"));
            formData.Add(new MultipartFormFileSection("fileThumbnail", fThumbnail, "thumbnail.png", "image/png"));
            //formData.Add(new MultipartFormFileSection("fileTexturesArchive", fArchive, "archive.zip", "application/x-zip-compressed"));
            formData.Add(new MultipartFormFileSection("fileTexturesArchive", fArchive, "archive.zip", "application/zip"));
            var op = Post(ServerUrl.Url + "Product/add", formData).SendWebRequest();
            Debug.Log(op.webRequest.downloadHandler.text);
        }
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
        public static byte[] DownloadProductTexturesArchive(int id)
        {
            var op = Get(ServerUrl.Url + "Download/textures/" + id).SendWebRequest();
            while (!op.isDone) { }
            return op.webRequest.downloadHandler.data;
        }
    }
}
