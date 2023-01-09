using Assets.Scripts.Network;
using Cysharp.Threading.Tasks;
using Shared.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Linq;
using System;

public class CartManagerScript : MonoBehaviour
{
    private static CartManagerScript instance;
    public static CartManagerScript Instance { get { return instance; } }
    private void Awake()
    {
        instance = this;
    }
    private List<CartItemDTO> _cart = new List<CartItemDTO>();

    

    //async void Start()
    //{
    //    var token = await CurrentSession.Instance.GetToken();
    //    if(token == null)
    //    {
    //        return;
    //    }
    //    var req = Network.CartApi.GetCart(token);
    //    var task = req.SendWebRequest().ToUniTask();
    //    UnityWebRequest
    //    = null;
    //    try
    //    {
    //        resp = await task;
    //    }
    //    catch (UnityWebRequestException)
    //    {
    //        throw;
    //    }
    //    _cart = JsonConvert.DeserializeObject<List<CartItemDTO>>(resp.downloadHandler.text);
    //}
    ////public List<CartItemDTO> Cart { get { return _cart; } }
    //public IEnumerable<CartItemDTO> GetItems() { return _cart; }
    //public int AddToCart(ProductDTO product, int count = 1)
    //{
    //    if (count < 1) throw new ArgumentOutOfRangeException("Count is 0 or negative");
    //    var productInCart = _cart.SingleOrDefault(p => p.ProductID == product.Id);
    //    if (productInCart != null) {
    //        productInCart.Quantity += count;
    //        return productInCart.Quantity;
    //    }
    //    _cart.Add(new CartItemDTO{ProductID=product.Id,Quantity = count });
    //    return count;
    //}
    //public bool RemoveFromCart(ProductDTO product)
    //{
    //    var productInCart = _cart.SingleOrDefault(p => p.ProductID == product.Id);
    //    if (productInCart == null) return false;
    //    _cart.Remove(productInCart);
    //    return true;
    //}
    //public int SubstractFromCart(ProductDTO product, int count = 1)
    //{
    //    if (count <= 0) throw new ArgumentOutOfRangeException("Count is 0 or negative");
    //    var productInCart = _cart.SingleOrDefault(p => p.ProductID == product.Id);
    //    if (productInCart == null) return -1;
    //    productInCart.Quantity -= count;
    //    if (productInCart.Quantity <= 0)
    //    {
    //        RemoveFromCart(product);
    //        return 0;
    //    }
    //    return productInCart.Quantity;
    //}
}
