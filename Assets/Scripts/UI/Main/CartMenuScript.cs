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

    //[SerializeField] Button ButtonExit;
    [SerializeField] GameObject CartItemPrefab;
    [SerializeField] ScrollRect ScrollRect;
    [SerializeField] TMP_Text TotalPrice;
    [SerializeField] Button ButtonCheckout;
    [SerializeField] GameObject WaitScreenPrefab;
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
    public void CalculatePrice()
    {
        decimal total = 0;
        foreach (Transform item in ScrollRect.content)
        {
            var script =  item.gameObject.GetComponent<CartItemScript>();
            total += script.price * script.quantity;
        }
        TotalPrice.text = total.ToString();
        totalPrice = total;
        TotalPrice.text = totalPrice.ToString();
    }
    private void Clear()
    {
        foreach(Transform item in ScrollRect.content)
        {
            Destroy(item.gameObject);
        }
        totalPrice = 0;
        TotalPrice.text = totalPrice.ToString();
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
        CalculatePrice();
    }
    public void Init()
    {
        instance = this;
        ButtonCheckout.onClick.AddListener(async () =>
        {
            //ButtonCheckout.interactable = false;
            if(ScrollRect.content == null || ScrollRect.content.childCount== 0) return;
            var req = Network.OrderApi.CreateOrder(await CurrentSession.Instance.GetToken());
            UnityWebRequest resp = null;
            int id;
            try
            {
                resp = await req.SendWebRequest().ToUniTask();
                var genericResponse = JsonConvert.DeserializeObject<GenericResponseDTO>(resp.downloadHandler.text);
                id = int.Parse(genericResponse.Message);
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
            _ = GoToCheckout(id);

        });
    }

    private async UniTask GoToCheckout(int id)
    {
        MenuScript.Instance.DisableBackShortcut();
        Application.OpenURL($"{Network.NetworkUtility.Url}checkout/{id}");
        var waitScreen = Instantiate(WaitScreenPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        waitScreen.transform.SetParent(transform, false);
        var waitScreenScript = waitScreen.GetComponent<WaitScreenScript>();
        waitScreenScript.ButtonCancel.gameObject.SetActive(true);
        waitScreenScript.ButtonCancel.onClick.AddListener(async () => {
            Destroy(waitScreenScript.gameObject);

            UnityWebRequest req = null;
            UnityWebRequest resp = null;
            try
            {
                req = Network.OrderApi.CancelOrder(await CurrentSession.Instance.GetToken(), id);
                resp = await req.SendWebRequest().ToUniTask();
                // It's not really that important if it fails or not
            }
            catch(UnityWebRequestException)
            {
                //throw;
            }
            finally { req?.Dispose(); resp?.Dispose(); MenuScript.Instance.EnableBackShortcut(); }
            
        });
        waitScreenScript.ButtonContinue.onClick.AddListener(() =>
        {
            Destroy(waitScreenScript.gameObject);
            Clear();
            MenuScript.Instance.EnableBackShortcut();
        });
        bool x = false;
        do
        {
            Debug.Log(waitScreen);
            if (waitScreen == null) return;
            x = await GetShouldStopWaiting(id, waitScreenScript);
            await UniTask.Delay(3500);
        }while(x == false);
        waitScreenScript.Icon.gameObject.SetActive(false);
        waitScreenScript.ButtonContinue.gameObject.SetActive(true);
        waitScreenScript.ButtonCancel.gameObject.SetActive(false);
        waitScreenScript.TextMessage.text = "Your order was completed successfuly";
    }

    private async UniTask<bool> GetShouldStopWaiting(int id, WaitScreenScript waitScreenScript)
    {
        UnityWebRequest req = Network.OrderApi.GetOrderStatus(id);
        UnityWebRequest resp = null;
        try
        {
            resp = await req.SendWebRequest().ToUniTask();
            var status = JsonConvert.DeserializeObject<OrderStatusDTO>(resp.downloadHandler.text);
            //Debug.Log(status.Status);
            if (status.Status == OrderStatusDTO.COMPLETED){
                return true;
            }
            return false;
            
        }
        catch (UnityWebRequestException)
        {
            //Destroy(script.gameObject);
            waitScreenScript.ButtonContinue.gameObject.SetActive(true);
            waitScreenScript.ButtonCancel.gameObject.SetActive(false);
            waitScreenScript.TextMessage.text = "An error occured while processing your order";
            throw;
        }
        finally
        {
            req?.Dispose(); resp?.Dispose(); 
        }
    }
}
