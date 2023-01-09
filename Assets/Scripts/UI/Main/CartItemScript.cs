using Assets.Scripts.Network;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Shared.DTO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
public class CartItemScript : MonoBehaviour
{
    [SerializeField] RawImage Image;
    [SerializeField] GameObject TextTitleGO;
    [SerializeField] GameObject TextPriceGO;
    [SerializeField] GameObject TextQuantityGO;
    [SerializeField] Button ButtonAdd;
    [SerializeField] Button ButtonSubstact;
    [SerializeField] Button ButtonRemove;
    [SerializeField] Button ButtonShowDetails;

    TextMeshProUGUI TextTitle;
    TextMeshProUGUI TextPrice;
    TextMeshProUGUI TextQuantity;

    public decimal price;
    public int quantity;

    //CartItemDTO _cartItem;
    //ProductDTO _product;
    UniTask modifyCartTask;
    public async UniTask Setup(CartItemDTO cartItem)
    {
        TextTitle = TextTitleGO.GetComponent<TextMeshProUGUI>();
        TextPrice = TextPriceGO.GetComponent<TextMeshProUGUI>();
        TextQuantity = TextQuantityGO.GetComponent<TextMeshProUGUI>();
        //var _cartItem = cartItem;
        ProductDTO _product = null;

        {
            UnityWebRequest reqProduct = Network.ProductApi.GetProduct(cartItem.ProductID);
            UnityWebRequest respProduct = null;
            
            try
            {
                respProduct = await reqProduct.SendWebRequest().ToUniTask();
                _product = JsonConvert.DeserializeObject<ProductDTO>(respProduct.downloadHandler.text);
            }
            catch (UnityWebRequestException)
            {
                Destroy(gameObject);
                throw;
            }
            finally
            {
                reqProduct?.Dispose();
                respProduct?.Dispose();
            }
        }

        {
            UnityWebRequest reqThumbnail = Network.ProductApi.GetThumbnail(cartItem.ProductID);
            UnityWebRequest respThumbnail = null;
            Texture2D texture;
            try
            {
                respThumbnail = await reqThumbnail.SendWebRequest().ToUniTask();
                texture = ((DownloadHandlerTexture)respThumbnail.downloadHandler).texture;
            }
            catch (UnityWebRequestException)
            {
                Destroy(gameObject);
                throw;
            }
            finally
            {
                reqThumbnail?.Dispose();
                respThumbnail?.Dispose();
            }
            Image.texture = texture;
        }

        TextTitle.text = _product.Title;
        TextPrice.text = _product.Price.ToString();
        TextQuantity.text = cartItem.Quantity.ToString();
        price = (decimal)_product.Price;
        quantity = cartItem.Quantity;
        gameObject.SetActive(true);

        ButtonShowDetails.onClick.AddListener(() =>
        {
            ProductDetailsScript.Instance.OpenMenu(_product);
        });
        
        ButtonAdd.onClick.AddListener( () =>
        {
            if(modifyCartTask.Status != UniTaskStatus.Succeeded)
            {
                return;
            }
            modifyCartTask = UniTask.Create(async () =>
            {
                NewQuantityDTO newQuantity = null;
                UnityWebRequest req = null;
                UnityWebRequest resp = null;
                try
                {
                    req = Network.CartApi.AddItem(new CartItemDTO { ProductID = _product.Id, Quantity = 1 }, await CurrentSession.Instance.GetToken());
                    resp = await req.SendWebRequest().ToUniTask();
                    newQuantity = JsonConvert.DeserializeObject<NewQuantityDTO>(resp.downloadHandler.text);
                }
                catch (UnityWebRequestException)
                {
                    modifyCartTask = UniTask.CompletedTask;
                    throw;
                }
                finally
                {
                    req?.Dispose();
                    resp?.Dispose();
                }
                TextQuantity.text = newQuantity.Quantity.ToString();
                quantity = newQuantity.Quantity;
                CartMenuScript.Instance.ChangePrice(price);
                modifyCartTask = UniTask.CompletedTask;
            });

            //int newQuantity = CartManagerScript.Instance.AddToCart(_product);
            //TextQuantity.text = newQuantity.ToString();
            //decimal price = (decimal)_product.Price;
            //CartMenuScript.Instance.ChangePrice(price);
        });
        ButtonSubstact.onClick.AddListener( () =>
        {
            if (modifyCartTask.Status != UniTaskStatus.Succeeded)
            {
                return;
            }
            modifyCartTask = UniTask.Create(async () =>
            {
                NewQuantityDTO newQuantity = null;
                UnityWebRequest req = null;
                UnityWebRequest resp = null;

                try
                {
                    req = Network.CartApi.RemoveItem(new CartItemDTO { ProductID = _product.Id, Quantity = 1 }, await CurrentSession.Instance.GetToken());
                    resp = await req.SendWebRequest().ToUniTask();
                    newQuantity = JsonConvert.DeserializeObject<NewQuantityDTO>(resp.downloadHandler.text);
                }
                catch (UnityWebRequestException)
                {
                    modifyCartTask = UniTask.CompletedTask;
                    throw;
                }
                finally
                {
                    req?.Dispose();
                    resp?.Dispose();
                }
                TextQuantity.text = newQuantity.Quantity.ToString();
                
                quantity = newQuantity.Quantity;
                CartMenuScript.Instance.ChangePrice(-price);
                if (newQuantity.Quantity <= 0)
                {
                    Destroy(gameObject);
                }
                modifyCartTask = UniTask.CompletedTask;
            });

            //int newQuantity = CartManagerScript.Instance.SubstractFromCart(_product);
            //CartMenuScript.Instance.ChangePrice(-price);
            //if (newQuantity > 0) TextQuantity.text = newQuantity.ToString();
            //else Destroy(gameObject);
        });
        ButtonRemove.onClick.AddListener( () =>
        {
            if (modifyCartTask.Status != UniTaskStatus.Succeeded)
            {
                return;
            }
            modifyCartTask = UniTask.Create(async () =>
            {
                NewQuantityDTO newQuantity = null;
                UnityWebRequest req = null;
                UnityWebRequest resp = null;
                try
                {
                    req = Network.CartApi.RemoveItem(cartItem, await CurrentSession.Instance.GetToken());
                    resp = await req.SendWebRequest().ToUniTask();
                    newQuantity = JsonConvert.DeserializeObject<NewQuantityDTO>(resp.downloadHandler.text);
                }
                catch (UnityWebRequestException)
                {
                    modifyCartTask = UniTask.CompletedTask;
                    throw;
                }
                finally
                {
                    req?.Dispose();
                    resp?.Dispose();
                }
                TextQuantity.text = newQuantity.Quantity.ToString();
                //CartMenuScript.Instance.ChangePrice(price);
                CartMenuScript.Instance.ChangePrice(-price * quantity);
                quantity = newQuantity.Quantity;
                
                if (newQuantity.Quantity <= 0)
                {
                    Destroy(gameObject);
                }
                modifyCartTask = UniTask.CompletedTask;
            });


            //bool isRemoved = CartManagerScript.Instance.RemoveFromCart(_product); 
            //if (isRemoved)
            //{
            //    Destroy(gameObject);
            //}
        });
        
    }


}
