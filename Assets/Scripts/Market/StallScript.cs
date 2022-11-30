using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Shared.DTO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public class StallScript : MonoBehaviour
{
    ProductSlotScript[] slots;

    private CategoryDTO Category;
    private int count;
    private int offset = 0;
    public void SetCategory(CategoryDTO category)
    {
        Category = category;
    }

    void Start()
    {
        slots = GetComponentsInChildren<ProductSlotScript>().Where(g => g.CompareTag("ProductSlot")).ToArray();
        count = slots.Length;
        GetProductsFromServer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ClickLeft()
    {

    }
    void ClickRight()
    {

    }

    async void GetProductsFromServer()
    {
        var req = Network.ProductApi.GetProducts(offset,count,Category.Name);
        var resp = await req.SendWebRequest().ToUniTask();
        var products = JsonConvert.DeserializeObject<ProductDTO[]>(resp.downloadHandler.text);
        if (products == null || products.Length == 0) return;
        for(int i = 0;i<slots.Length;i++)
        {
            if (i >= products.Length)
            {
                slots[i].gameObject.SetActive(false);
            }
            else
            {
                slots[i].gameObject.SetActive(true);
                slots[i].SetProduct(products[i]);
                slots[i].DownloadThumbnailAndSet();
            }
        }
    }
}
