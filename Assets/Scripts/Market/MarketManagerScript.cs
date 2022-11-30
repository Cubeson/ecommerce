using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Shared;
using Shared.DTO;

public class MarketManagerScript : MonoBehaviour
{
    [SerializeField]
    GameObject StallPrefab;
    async void Start()
    {
        var req = Network.ProductApi.GetCategories();
        var resp = await req.SendWebRequest().ToUniTask();
        var categories = JsonConvert.DeserializeObject<CategoryDTO[]>(resp.downloadHandler.text);
        Vector3 pos = new Vector3(0, 0.778f, 0);
        foreach (var category in categories)
        {
            var stall = Instantiate(StallPrefab, pos, Quaternion.identity);
            var stallScript = stall.GetComponent<StallScript>();
            stallScript.SetCategory(category);
            pos.z += 5f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
