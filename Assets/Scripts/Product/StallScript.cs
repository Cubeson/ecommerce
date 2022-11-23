using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class StallScript : MonoBehaviour
{
    ProductSlotScript[] slots;
    [SerializeField] string category;
    void Start()
    {
        slots = GetComponentsInChildren<ProductSlotScript>().Where(g => g.CompareTag("ProductSlot")).ToArray();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GetProductsFromServer()
    {

    }
}
