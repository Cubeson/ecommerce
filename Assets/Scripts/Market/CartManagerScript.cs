using Assets.Scripts.Network;
using Cysharp.Threading.Tasks;
using Shared.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Linq;

public class CartManagerScript : MonoBehaviour
{
    private static CartManagerScript instance;
    public static CartManagerScript Instance { get { return instance; } }
    private void Awake()
    {
        instance = this;
    }
    private List<CartItemDTO> _cart = new List<CartItemDTO>();
    async void Start()
    {
        var token = CurrentSession.GetInstance().GetToken();
        if(token == null)
        {
            return;
        }
        var req = Network.CartApi.GetCart(token);
        var task = req.SendWebRequest().ToUniTask();
        UnityWebRequest resp = null;
        try
        {
            resp = await task;
        }
        catch (UnityWebRequestException)
        {
            throw;
        }
        _cart = JsonConvert.DeserializeObject<List<CartItemDTO>>(resp.downloadHandler.text);
    }
    public List<CartItemDTO> Cart { get { return _cart; } }
    public void AddToCart(ProductDTO product)
    {
        var productInCart = Cart.SingleOrDefault(p => p.ProductID == product.Id);
        if (productInCart != null) {
            productInCart.Quantity++;
            return;
        }
        _cart.Add(new CartItemDTO { ProductID=product.Id,Quantity=1});
    }

}
