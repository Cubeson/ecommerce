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
        //file = File.Create("temp/file.txt");
        //byte[] x1 = Encoding.ASCII.GetBytes("Hello World");
        //file.Write(x1, 0, x1.Length);
        //file.Close();
        //
        //string meshFile = "temp/file.txt";
        //file = File.OpenRead(meshFile);
        //byte[] x2 = new byte[20];
        //file.Read(x2, 0, 5);
        //Debug.Log(Encoding.ASCII.GetString(x2));
        //Debug.Log(Path.GetFullPath(meshFile));

        //var product = ProductEndpoint.

        var bytes = ProductEndpoint.DownloadProduct(1);
        file = File.Create(dir+"file.fbx");
        file.Write(bytes,0,bytes.Length);
        file.Close();
        var ob = MeshImporter.Load(dir+ "file.fbx");

    }

}
