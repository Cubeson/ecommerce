using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Newtonsoft.Json;
using Network;

public class OmegaScript : MonoBehaviour
{
    public GameObject plane;

    private string RequestImagesURL()
    {
        //WebRequest request = WebRequest.Create("https://jaklew.pl/app/GET_images/");
        WebRequest request = WebRequest.Create("https://localhost:7028/items/all");
        request.Credentials = CredentialCache.DefaultCredentials;

        WebResponse response = request.GetResponse();

        using (Stream dataStream = response.GetResponseStream())
        {
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            Debug.Log(responseFromServer);
            response.Close();
            return responseFromServer;
        }
        response.Close();
    }
    void Start()
    {

        //var urls = RequestImagesURL();
        //using(WebClient webclient = new WebClient())
        //{
        //    var images = JsonConvert.DeserializeObject<Images[]>(urls); 
        //    foreach (var image in images)
        //    {
        //        try
        //        {
        //            byte[] data = webclient.DownloadData(image.url);
        //            Texture2D tx = new Texture2D(1, 1);
        //            ImageConversion.LoadImage(tx, data);
        //        }
        //        catch (WebException ex)
        //        {
        //            Debug.Log(ex);
        //        }
        //    }
        //}

        void Update()
        {
            //if (!Input.GetKeyDown(KeyCode.S)) return;

            //UserEndpoint.CreateUser();
        }
    }
}