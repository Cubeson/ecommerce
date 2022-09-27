using Network;
using Network.DTO;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityMeshImporter;
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
            /*
             * Thumbnail
             */
            {
                string fPath = "downloads/" + products[i].Name + "/" + StaticStrings.FilethumbnailString + ".png";
                byte[] imageData = null;
                if (File.Exists(fPath))
                {
                    imageData = File.ReadAllBytes(fPath);
                }
                else
                {
                    imageData = ProductEndpoint.DownloadProductThumbnail(products[i].Id);
                    FileStream imageFile = File.OpenWrite(fPath);
                    imageFile.Write(imageData, 0, imageData.Length);
                    imageFile.Close();
                }
                var tx = new Texture2D(1, 1);
                tx.LoadImage(imageData);
                var sprite = Sprite.Create(tx, new Rect(0, 0, tx.width, tx.height), new Vector2(tx.width / 2, tx.height / 2));
                SetSlotThumbnail(slots[i], sprite);
            }
            /*
             * 3D Model
             */
            {
                string fPath = "downloads/" + products[i].Name + "/" + StaticStrings.FileModelString + "." + products[i].FileFormat;
                byte[] modelData = null;
                if (File.Exists(fPath))
                {
                    //modelData = File.ReadAllBytes(fPath);
                }
                else
                {
                    modelData = ProductEndpoint.DownloadProductModel(products[i].Id);
                    FileStream imageFile = File.OpenWrite(fPath);
                    imageFile.Write(modelData, 0, modelData.Length);
                    imageFile.Close();
                }
                Debug.Log(fPath);
                MeshImporter.Load(fPath);
            }

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

    private void HideSlots(int count)
    {
        for(int i = slots.Count - 1; i >= slots.Count - count; i--)
        {
            slots[i].SetActive(false);
        }
    }
}
