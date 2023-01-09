using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Shared.DTO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using Shared.SortOrderDB;
using UnityEngine.Networking;

public class StallScript : MonoBehaviour
{
    [SerializeField] GameObject CategoryName;
    ProductSlotScript[] slots;
    private CategoryDTO Category;
    private int count;
    private int offset = 0;
    private decimal _minPrice;
    private decimal _maxPrice;

    private string _nameFilter = "";
    private SortOrderDB _sortOrder = SortOrderDB.DateModified_Desc;

    public void SetFilter(SortOrderDB sortOrder, string nameFilter, decimal minPrice, decimal maxPrice, bool keepOffset = false)
    {
        if(!keepOffset)
            offset = 0;
        _sortOrder = sortOrder;
        _nameFilter = nameFilter;
        _minPrice = minPrice;
        _maxPrice = maxPrice;

    }

    public void SetCategory(CategoryDTO category)
    {
        Category = category;
    }
    void Start()
    {
        CategoryName.GetComponent<TextMeshPro>().text = Category.Name;
        slots = GetComponentsInChildren<ProductSlotScript>().Where(g => g.CompareTag("ProductSlot")).ToArray();
        count = slots.Length;
        GetProductsFromServer();
    }
    UniTask currentTask;
    public void Previous()
    {
        //Debug.Log($"Previous {Category.Name}");
        if (!(currentTask.Status == UniTaskStatus.Succeeded)) return;
        if (offset == 0) return;
        currentTask = UniTask.Create(async () =>
        {
            var products = await RequestProducts(offset - count);
            if (products != null && products.Length > 0)
            {
                DisableAllSlots();
                SetProducts(products);
                offset -= count;
            }
            return UniTask.CompletedTask;
        });


    }
    public void Next()
    {
        //Debug.Log($"Next {Category.Name}");
        if (!(currentTask.Status == UniTaskStatus.Succeeded)) return;
        currentTask = UniTask.Create(async () =>
        {
            var products = await RequestProducts(offset + count);
            if (products != null && products.Length > 0)
            {
                DisableAllSlots();
                SetProducts(products);
                offset += count;
            }
            return UniTask.CompletedTask;
        });
    }

    async UniTask<ProductDTO[]> RequestProducts(int offset)
    {
        UnityWebRequest req = Network.ProductApi.GetProducts(offset, count, Category.Name, _sortOrder, _nameFilter,_minPrice,_maxPrice);
        UnityWebRequest resp = null;
        try
        {
            resp = await req.SendWebRequest().ToUniTask();
            return JsonConvert.DeserializeObject<ProductDTO[]>(resp.downloadHandler.text);
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

    }
    void DisableAllSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].gameObject.SetActive(false);
        }
    }
    void SetProducts(ProductDTO[] products)
    {
        DisableAllSlots();
        for (int i = 0; i < slots.Length; i++)
        {
            if (i < products.Length)
            {
                slots[i].gameObject.SetActive(true);
                slots[i].SetProduct(products[i]);
                _ = slots[i].DownloadThumbnailAndSet();
            }
        }
    }

    public void GetProductsFromServer()
    {
        if (!(currentTask.Status == UniTaskStatus.Succeeded)) return;
        currentTask = UniTask.Create(async () =>
        {
            //var req = Network.ProductApi.GetProducts(offset, count, Category.Name, _sortOrder, _nameFilter);
            //var resp = await req.SendWebRequest().ToUniTask();
            //var products = JsonConvert.DeserializeObject<ProductDTO[]>(resp.downloadHandler.text);
            var products = await RequestProducts(offset);
            DisableAllSlots();
            if (products == null || products.Length == 0) return;
            for (int i = 0; i < slots.Length; i++)
            {
                if (i < products.Length)
                {
                    slots[i].gameObject.SetActive(true);
                    slots[i].SetProduct(products[i]);
                    _ = slots[i].DownloadThumbnailAndSet();
                }
            }
        });


    }
}
