using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisabledMenuInitScript : MonoBehaviour
{
    [SerializeField] GameObject ProductDetails;
    [SerializeField] GameObject CartMenu;
    void Start()
    {
        ProductDetails.GetComponent<ProductDetailsScript>().Init();
        CartMenu.GetComponent<CartMenuScript>().Init();
    }

}
