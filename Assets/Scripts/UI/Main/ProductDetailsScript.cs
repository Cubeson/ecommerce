using Cysharp.Threading.Tasks;
using Shared.DTO;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.IO.Compression;
using System.IO;
using Assets.Scripts.Network;

public class ProductDetailsScript : MonoBehaviour
{
    static ProductDetailsScript instance;
    public static ProductDetailsScript Instance { get { return instance; } }

    [SerializeField] Button ButtonNext;
    [SerializeField] Button ButtonPrevious;
    [SerializeField] Button ButtonAddToCart;
    [SerializeField] Button ButtonExit;
    [SerializeField] Button ButtonView3DModel;
    [SerializeField] RawImage Image;
    [SerializeField] GameObject TextTitleGO;
    [SerializeField] GameObject TextDescriptionGO;
    [SerializeField] GameObject TextPriceGO;
    [SerializeField] GameObject ModelView;

    TextMeshProUGUI TextTitle;
    TextMeshProUGUI TextDescription;
    TextMeshProUGUI TextPrice;
    
    ProductDTO productDTO;
    List<Texture2D> images;

    int imageIndex = 0;

    //public async void OpenMenu(int productId)
    public async void OpenMenu(ProductDTO product)
    {
        MenuScript.Instance.PushMenu(gameObject);
        images = new List<Texture2D>();

        //var req = Network.ProductApi.GetProduct(productId);
        //var resp = await req.SendWebRequest().ToUniTask();
        //var product = JsonConvert.DeserializeObject<ProductDTO>(resp.downloadHandler.text);
        productDTO = product;

        TextTitle.text = product.Title;
        TextDescription.text = product.Description;
        TextPrice.text = product.Price.ToString();

        var respThumbnail = await Network.ProductApi.GetThumbnail(productDTO.Id).SendWebRequest().ToUniTask();
        var textureThumbnail = ((DownloadHandlerTexture)respThumbnail.downloadHandler).texture;
        images.Add(textureThumbnail);
        Image.texture = textureThumbnail;

        var respPictures = await Network.ProductApi.GetPictures(productDTO.Id).SendWebRequest().ToUniTask();
        var archive = new ZipArchive(new MemoryStream(respPictures.downloadHandler.data));
        foreach(var entry in archive.Entries)
        {
            var st = entry.Open();
            var fbytes = ReadFully(st);
            Texture2D texture = new Texture2D(1,1);
            texture.LoadImage(fbytes);
            texture.Apply();
            images.Add(texture);
        }

    }

    public void Init()
    {
        instance = this;

        var container = ModelView.transform.Find("Container");
        var modelViewControllerScript = container.GetComponent<ModelViewControllerScript>();

        TextTitle = TextTitleGO.GetComponent<TextMeshProUGUI>();
        TextDescription = TextDescriptionGO.GetComponent<TextMeshProUGUI>();
        TextPrice = TextPriceGO.GetComponent<TextMeshProUGUI>();
        ButtonExit.onClick.AddListener(() =>
        {
            MenuScript.Instance.PopMenu();
        });
        ButtonAddToCart.onClick.AddListener(() =>
        {
            //var req = Network.CartApi.AddItem(new CartItemDTO { ProductID = productDTO.Id, Quantity = 1 }, await CurrentSession.Instance.GetToken());
            CartManagerScript.Instance.AddToCart(productDTO); 
        });
        ButtonNext.onClick.AddListener(() =>
        {
            if (imageIndex >= images.Count - 1)
            {
                imageIndex = 0;
            }
            else imageIndex++;
            var image = images[imageIndex];
            Image.texture = image;
        });
        ButtonPrevious.onClick.AddListener(() =>
        {
            if (imageIndex == 0)
            {
                imageIndex = images.Count - 1;
            }
            else imageIndex--;
            var image = images[imageIndex];
            Image.texture = image;
        });
        ButtonView3DModel.onClick.AddListener(async () =>
        {
            MenuScript.Instance.PushMenu(ModelView);

            var req = Network.ProductApi.GetModel(productDTO.Id).SendWebRequest().ToUniTask();
            var resp = await req;
            Debug.Log(productDTO.Id);
            modelViewControllerScript.SetModel(resp.downloadHandler.data);
        });
    }
    public static byte[] ReadFully(Stream input)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }

}
