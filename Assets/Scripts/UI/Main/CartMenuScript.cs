using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CartMenuScript : MonoBehaviour
{
    [SerializeField] Button ButtonExit;
    [SerializeField] GameObject CartItemPrefab;
    [SerializeField] ScrollRect ScrollRect;
    void Start()
    {
        
    }
    private void OnEnable()
    {
        Open();
    }
    private void Clear()
    {
        foreach(Transform item in ScrollRect.content)
        {
            Destroy(item.gameObject);
        }
    }
    public void Open()
    {
        Clear();
        var cart = CartManagerScript.Instance.GetItems();
        foreach (var item in cart) 
        {
            var go = Instantiate(CartItemPrefab);
            go.transform.SetParent(ScrollRect.content,false);
            go.SetActive(false);
            go.GetComponent<CartItemScript>().Setup(item);
            go.SetActive(true);
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
