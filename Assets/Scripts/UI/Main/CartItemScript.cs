using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Shared.DTO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

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

    //CartItemDTO _cartItem;
    //ProductDTO _product;
    public async void Setup(CartItemDTO cartItem)
    {
        TextTitle = TextTitleGO.GetComponent<TextMeshProUGUI>();
        TextPrice = TextPriceGO.GetComponent<TextMeshProUGUI>();
        TextQuantity = TextQuantityGO.GetComponent<TextMeshProUGUI>();
        var _cartItem = cartItem;
        var req = Network.ProductApi.GetProduct(cartItem.ProductID);
        var resp = await req.SendWebRequest().ToUniTask();
        var _product = JsonConvert.DeserializeObject<ProductDTO>(resp.downloadHandler.text);

        var reqThumbnail = Network.ProductApi.GetThumbnail(cartItem.ProductID);
        var respThumbnail = await reqThumbnail.SendWebRequest().ToUniTask();
        var texture = ((DownloadHandlerTexture)respThumbnail.downloadHandler).texture;

        Image.texture = texture;
        TextTitle.text = _product.Title;
        TextPrice.text = _product.Price.ToString();
        TextQuantity.text = cartItem.Quantity.ToString();
        gameObject.SetActive(true);

        ButtonShowDetails.onClick.AddListener(() =>
        {
            ProductDetailsScript.Instance.OpenMenu(_product);
        });
        ButtonAdd.onClick.AddListener(() =>
        {
            int newQuantity = CartManagerScript.Instance.AddToCart(_product);
            TextQuantity.text = newQuantity.ToString();
        });
        ButtonSubstact.onClick.AddListener(() =>
        {
            int newQuantity = CartManagerScript.Instance.SubstractFromCart(_product);
            if(newQuantity > 0) TextQuantity.text = newQuantity.ToString();
            else Destroy(gameObject);
        });
        ButtonRemove.onClick.AddListener(() =>
        {
            bool isRemoved = CartManagerScript.Instance.RemoveFromCart(_product);
            if (isRemoved)
            {
                Destroy(gameObject);
            }
        });

        
    }


}
