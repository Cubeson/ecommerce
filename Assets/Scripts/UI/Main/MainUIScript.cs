using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainUIScript : MonoBehaviour
{
    [SerializeField] GameObject CartGO;
    [SerializeField] Button ButtonOpenCart;
    void Start()
    {
        ButtonOpenCart.onClick.AddListener(() =>
        {
            MenuScript.Instance.PushMenu(CartGO);
        });
    }

}
