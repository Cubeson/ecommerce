using Network;
using Network.DTO;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class SlotScript : MonoBehaviour
{
    public ProductDTO product;
    void Start()
    {
        var btn = gameObject.GetComponent<Button>();
        btn.onClick.AddListener(Fun);
    }
    private void Fun()
    {
        string metaPath = "downloads/" + product.Name + "/" + ".meta";
        byte[] bytes = File.ReadAllBytes(metaPath);
        ProductMetadata metadata = JsonConvert.DeserializeObject<ProductMetadata>(Encoding.ASCII.GetString(bytes));
        if(metadata.FullDownload == 0)
        {
            DownloadFiles();
            var meta = File.OpenWrite(metaPath);
            string json = JsonConvert.SerializeObject(new ProductMetadata(metadata.Date,1));
            byte[] jsonBytes = Encoding.ASCII.GetBytes(json);
            meta.Write(jsonBytes, 0, jsonBytes.Length);
        }
        foreach(Transform child in CurrentModel.GameObject.transform)
        {
            Destroy(child.gameObject);
        }
        //var GO = MeshImporter.Load("downloads/"+product.Name+"/"+StaticStrings.FileModelString +"."+product.FileFormat);
        //GO.transform.parent = CurrentModel.GameObject.transform;
    }
    private void DownloadFiles()
    {
        {
            string fPath = "downloads/" + product.Name + "/" + StaticStrings.FileModelString + "." + product.FileFormat;
            byte[] modelData = null;
            if (File.Exists(fPath))
            {
                //modelData = File.ReadAllBytes(fPath);
            }
            else
            {
                modelData = ProductEndpoint.DownloadProductModel(product.Id);
                FileStream modelFile = File.OpenWrite(fPath);
                modelFile.Write(modelData, 0, modelData.Length);
                modelFile.Close();
            }
        }
        {
            var bytes = ProductEndpoint.DownloadProductTexturesArchive(product.Id);
            var archive = new ZipArchive(new MemoryStream(bytes));
            archive.ExtractToDirectory("downloads/"+product.Name);

            
        }

    }
}
