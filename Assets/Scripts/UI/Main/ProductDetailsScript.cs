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
    [SerializeField] TMP_Text InStockValue;

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

        ProductDTO _product = null;
        {
            UnityWebRequest reqProduct = Network.ProductApi.GetProduct(product.Id);
            UnityWebRequest respProduct = null;
            try
            {
                respProduct = await reqProduct.SendWebRequest().ToUniTask();
                _product = JsonConvert.DeserializeObject<ProductDTO>(respProduct.downloadHandler.text);
            }
            catch(UnityWebRequestException)
            {
                throw;
            }
            finally
            {
                reqProduct?.Dispose();
                respProduct?.Dispose();
            }
        }


        productDTO = _product;
        TextTitle.text = _product.Title;
        TextDescription.text = _product.Description;
        TextPrice.text = _product.Price.ToString();
        InStockValue.text = _product.InStock.ToString();

        bool isInCart = false;
        UnityWebRequest reqIsInCart = null;
        UnityWebRequest respIsInCart = null;
        try
        {
            reqIsInCart = Network.CartApi.IsProductInCart(_product.Id, await CurrentSession.Instance.GetToken());
            respIsInCart = await reqIsInCart.SendWebRequest().ToUniTask();
            var obj = JsonConvert.DeserializeObject<IsInCartDTO>(respIsInCart.downloadHandler.text);
            isInCart = obj.IsInCart;
        }
        catch (UnityWebRequestException)
        {
            throw;
        }
        finally
        {
            reqIsInCart?.Dispose();
            respIsInCart?.Dispose();
        }
        ButtonAddToCart.interactable = !isInCart;
        Texture2D textureThumbnail = null;

        {
            UnityWebRequest reqThumbnail = Network.ProductApi.GetThumbnail(productDTO.Id);
            UnityWebRequest respThumbnail = null;
            try
            {
                respThumbnail = await reqThumbnail.SendWebRequest().ToUniTask();
                textureThumbnail = ((DownloadHandlerTexture)respThumbnail.downloadHandler).texture;
            }
            catch (UnityWebRequestException)
            {
                throw;
            }
            finally
            {
                reqThumbnail?.Dispose();
                respThumbnail?.Dispose();
            }
        }

        images.Add(textureThumbnail);
        Image.texture = textureThumbnail;

        ZipArchive archive = null;
        {
            UnityWebRequest reqPictures = Network.ProductApi.GetPictures(productDTO.Id);
            UnityWebRequest respPictures = null;
            try
            {
                respPictures = await reqPictures.SendWebRequest().ToUniTask();
                archive = new ZipArchive(new MemoryStream(respPictures.downloadHandler.data));
            }
            catch (UnityWebRequestException)
            {
                throw;
            }
            finally
            {
                reqPictures?.Dispose();
                respPictures?.Dispose();
            }
        }
        

        
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
    UniTask addToCartTask;
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
        ButtonAddToCart.onClick.AddListener( () =>
        {
            if (addToCartTask.Status != UniTaskStatus.Succeeded) return;
            UnityWebRequest req = null;
            UnityWebRequest resp = null;
            addToCartTask = UniTask.Create(async () =>
            {
                try
                {
                    req = Network.CartApi.AddItem(new CartItemDTO { ProductID = productDTO.Id, Quantity = 1 }, await CurrentSession.Instance.GetToken());
                    resp = await req.SendWebRequest().ToUniTask();
                }
                catch (UnityWebRequestException)
                {
                    throw;
                }
                finally
                {
                    req?.Dispose();
                    resp?.Dispose();
                }
                ButtonAddToCart.interactable = false;
            });

            //CartManagerScript.Instance.AddToCart(productDTO); 
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
            UnityWebRequest req = Network.ProductApi.GetModel(productDTO.Id);
            UnityWebRequest resp = null;
            try
            {
                resp = await req.SendWebRequest().ToUniTask();
                modelViewControllerScript.SetModel(resp.downloadHandler.data);
            }
            catch(UnityWebRequestException)
            {
                throw;
            }
            finally
            {
                req?.Dispose();
                resp?.Dispose();
            }

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
