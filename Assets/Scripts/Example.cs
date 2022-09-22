using Assimp;
using Network;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityMeshImporter;


public class Example : MonoBehaviour
{
    private static readonly string dir = "tempfiles/";
    void Start()
    {
        Directory.CreateDirectory(dir);
        FileStream file;
        var productDTO = ProductEndpoint.GetProductInfo(1);
        if (productDTO == null) return;
        var name = productDTO.Name;
        var bytes = ProductEndpoint.DownloadProduct(1);

        file = File.Create(dir + name + ".fbx");
        file.Write(bytes,0,bytes.Length);
        file.Close();
        var ob = MeshImporter.Load(dir + name + ".fbx");

    }

}
