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

    TextMeshProUGUI TextTitle;
    TextMeshProUGUI TextPrice;
    TextMeshProUGUI TextQuantity;

    public CartItemDTO _cartItem;

    public async void GetProductFromServer(CartItemDTO cartItem)
    {
        TextTitle = TextTitleGO.GetComponent<TextMeshProUGUI>();
        TextPrice = TextPriceGO.GetComponent<TextMeshProUGUI>();
        TextQuantity = TextQuantityGO.GetComponent<TextMeshProUGUI>();
        _cartItem = cartItem;
        var req = Network.ProductApi.GetProduct(cartItem.ProductID);
        var resp = await req.SendWebRequest().ToUniTask();
        var product = JsonConvert.DeserializeObject<ProductDTO>(resp.downloadHandler.text);

        var reqThumbnail = Network.ProductApi.GetThumbnail(cartItem.ProductID);
        var respThumbnail = await reqThumbnail.SendWebRequest().ToUniTask();
        var texture = ((DownloadHandlerTexture)respThumbnail.downloadHandler).texture;

        Image.texture = texture;
        TextTitle.text = product.Title;
        TextPrice.text = product.Price.ToString();
        TextQuantity.text = cartItem.Quantity.ToString();
        gameObject.SetActive(true);
    }


}
