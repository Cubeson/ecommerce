using Network;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;


public class Example : MonoBehaviour
{
    private static readonly string dir = "tempfiles/";
    void Start()
    {
        Application.OpenURL("https://www.youtube.com/watch?v=dQw4w9WgXcQ");

        //ProductEndpoint.Temp();
        //Directory.CreateDirectory(dir);
        //FileStream file;
        //var productDTO = ProductEndpoint.GetProductInfo(1);
        //if (productDTO == null) return;
        //var name = productDTO.Name;
        //var bytes = ProductEndpoint.DownloadProductModel(1);
        //
        //file = File.Create(dir + name + ".fbx");
        //file.Write(bytes,0,bytes.Length);
        //file.Close();
        //var ob = MeshImporter.Load(dir + name + ".fbx");

    }

}
