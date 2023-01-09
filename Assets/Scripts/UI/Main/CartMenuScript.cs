using Assets.Scripts.Network;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Shared.DTO;
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

        UnityWebRequest req = null;
        UnityWebRequest resp = null;
        List<CartItemDTO> cart = null;
        try
        {
            req = Network.CartApi.GetCart(await CurrentSession.Instance.GetToken());
            resp = await req.SendWebRequest().ToUniTask();
            cart = JsonConvert.DeserializeObject<List<CartItemDTO>>(resp.downloadHandler.text);
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
        List<UniTask> tasks = new(cart.Count());
        foreach (var item in cart)
        {
            var go = Instantiate(CartItemPrefab);
            go.transform.SetParent(ScrollRect.content, false);
            go.SetActive(false);
            tasks.Add(go.GetComponent<CartItemScript>().Setup(item));
            go.SetActive(true);
        }
        await UniTask.WhenAll(tasks);
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
                //var items = CartManagerScript.Instance.GetItems();
                //if (items.Count() == 0) return UniTask.CompletedTask;

                var req = Network.OrderApi.CreateOrder(await CurrentSession.Instance.GetToken());
                UnityWebRequest resp = null;
                string id;
                try
                {
                    resp = await req.SendWebRequest().ToUniTask();
                    var genericResponse = JsonConvert.DeserializeObject<GenericResponseDTO>(resp.downloadHandler.text);
                    id = genericResponse.Message;
                }
                catch(UnityWebRequestException)
                {
                    return UniTask.CompletedTask;
                }
                finally
                {
                    req?.Dispose();
                    resp?.Dispose();
                }
                Application.OpenURL($"{Network.NetworkUtility.Url}checkout/{id}");
                return UniTask.CompletedTask;
            });


        });
    }
}
