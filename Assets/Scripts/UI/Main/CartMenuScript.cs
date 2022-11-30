using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CartMenuScript : MonoBehaviour
{
    [SerializeField] Button ButtonExit;
    [SerializeField] GameObject CartItemPrefab;
    void Start()
    {
        
    }
    private void OnEnable()
    {
        Open();
    }
    public void Open()
    {
        int x = -350;
        int y = 250;
        //MenuScript.Instance.OpenMenu();
        var cart = CartManagerScript.Instance.Cart;
        foreach (var item in cart) 
        {
            var go = Instantiate(CartItemPrefab,new Vector3(x,y,0),Quaternion.identity);
            y -= 250;
            go.transform.SetParent(transform,false);
            go.SetActive(false);
            go.GetComponent<CartItemScript>().GetProductFromServer(item);
        }
    }

    public void Init()
    {
        ButtonExit.onClick.AddListener(() =>
        {
            MenuScript.Instance.PopMenu();
        });
    }
}
