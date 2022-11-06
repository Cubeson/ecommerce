using Network;
using Network.DTO;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class GalleryMainScript : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> slots = new List<GameObject> (8);

    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.tag.Equals("GallerySlot"))
            {
                slots.Add(child.gameObject);
            }
        }
        fun();

    }
    private void OnEnable()
    {
        
    }

    private void fun()
    {
        var products = ProductEndpoint.GetProducts(slots.Count);
        //Debug.Log(Directory.GetCurrentDirectory());
        HideSlots(slots.Count - products.Length);
        for(int  i = 0; i < products.Length; i++)
        {
            SetSlotName(slots[i], products[i].Name);
            Directory.CreateDirectory("downloads/" + products[i].Name);
            byte[] imageData;
            string fPath = "downloads/" + products[i].Name + "/" + StaticStrings.FilethumbnailString + ".png";
            if (!File.Exists("downloads/" + products[i].Name + "/.meta"))
            {
                var meta = File.Create("downloads/" + products[i].Name + "/.meta");
                ProductMetadata metadata = new ProductMetadata(DateTime.Now, 0);
                string json = JsonConvert.SerializeObject(metadata);
                byte[] bytes = Encoding.ASCII.GetBytes(json);
                meta.Write(bytes, 0, bytes.Length);
                meta.Close();

                imageData = ProductEndpoint.DownloadProductThumbnail(products[i].Id);
                FileStream imageFile = File.OpenWrite(fPath);
                imageFile.Write(imageData, 0, imageData.Length);
                imageFile.Close();
            }
            else
            {
                imageData = File.ReadAllBytes(fPath);
            }
            var tx = new Texture2D(1, 1);
            tx.LoadImage(imageData);
            var sprite = Sprite.Create(tx, new Rect(0, 0, tx.width, tx.height), new Vector2(tx.width / 2, tx.height / 2));
            SetSlotThumbnail(slots[i], sprite);
            SetSlotProduct(slots[i], products[i]);
        }
    }
    private void SetSlotName(GameObject slot, string name)
    {   
        slot.transform.Find("Name").gameObject.GetComponent<Text>().text = name;
    }
    private void SetSlotThumbnail(GameObject slot, Sprite sprite)
    {
        slot.GetComponent<Image>().overrideSprite = sprite;
    }
    private void SetSlotProduct(GameObject slot, ProductDTO product)
    {
        slot.GetComponent<SlotScript>().product = product;
    }

    private void HideSlots(int count)
    {
        for(int i = slots.Count - 1; i >= slots.Count - count; i--)
        {
            slots[i].SetActive(false);
        }
    }
}
