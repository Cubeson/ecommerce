using Assets.Scripts.Network;
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CartMenuScript : MonoBehaviour
{
    private static CartMenuScript instance;
    public static CartMenuScript Instance { get { return instance; } }

    [SerializeField] Button ButtonExit;
    [SerializeField] GameObject CartItemPrefab;
    [SerializeField] ScrollRect ScrollRect;
    [SerializeField] TMP_Text TotalPrice;
    [SerializeField] Button ButtonCheckout;
    private decimal totalPrice = 0;
    public void ChangePrice(decimal price)
    {
        totalPrice += price;
        TotalPrice.text = totalPrice.ToString();
    }
    private async void OnEnable()
    {
        await Open();
    }
    public void RecalculatePrice()
    {
        decimal total = 0;
        foreach (Transform item in ScrollRect.content)
        {
            var script =  item.gameObject.GetComponent<CartItemScript>();
            total += script.price * script.quantity;
        }
        TotalPrice.text = total.ToString();
        totalPrice = total;
    }
    private void Clear()
    {
        foreach(Transform item in ScrollRect.content)
        {
            Destroy(item.gameObject);
        }
    }
    public async UniTask Open()
    {
        Clear();

        //Network.CartApi.GetCart(await CurrentSession.Instance.GetToken());

        var cart = CartManagerScript.Instance.GetItems();
        List<UniTask> tasks = new(cart.Count());
        foreach (var item in cart) 
        {
            var go = Instantiate(CartItemPrefab);
            go.transform.SetParent(ScrollRect.content,false);
            go.SetActive(false);
            tasks.Add(go.GetComponent<CartItemScript>().Setup(item));
            go.SetActive(true);
        }
        await UniTask.WhenAll(tasks.ToArray());
        RecalculatePrice();
    }
    UniTask checkoutTask;

    public void Init()
    {
        instance = this;
        ButtonExit.onClick.AddListener(() =>
        {
            MenuScript.Instance.PopMenu();
        });
        ButtonCheckout.onClick.AddListener(() =>
        {
            if (checkoutTask.Status != UniTaskStatus.Succeeded) return;
            checkoutTask = UniTask.Create( async () =>
            {
                var items = CartManagerScript.Instance.GetItems();
                if (items.Count() == 0) return UniTask.CompletedTask;

                var req = Network.OrderApi.CreateOrder(await CurrentSession.Instance.GetToken());
                UnityWebRequest resp = null;
                try
                {
                    resp = await req.SendWebRequest().ToUniTask();
                }
                catch(UnityWebRequestException)
                {
                    return UniTask.CompletedTask;
                }
                var id = resp.downloadHandler.text;
                Application.OpenURL($"{Network.NetworkUtility.Url}checkout/{id}");
                return UniTask.CompletedTask;
            });


        });
    }
}
