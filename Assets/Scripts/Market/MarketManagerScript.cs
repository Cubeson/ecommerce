using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Shared;
using Shared.DTO;

public class MarketManagerScript : MonoBehaviour
{
    static MarketManagerScript instance;
    public static MarketManagerScript Instance { get { return instance; } }
    private void Awake()
    {
        instance = this;
    }

    [SerializeField]GameObject MarketStallsGO;
    StallPlacerScript stallPlacerScript;
    async void Start()
    {
        stallPlacerScript = MarketStallsGO.GetComponent<StallPlacerScript>();

        var req = Network.ProductApi.GetCategories();
        var resp = await req.SendWebRequest().ToUniTask();
        var categories = JsonConvert.DeserializeObject<CategoryDTO[]>(resp.downloadHandler.text);
        await UniTask.NextFrame();
        SetupStalls(categories);
    }
    public void ClearStalls()
    {
        stallPlacerScript.ClearAll();
    }
    public void SetupStalls(CategoryDTO[] categories)
    {
        ClearStalls();
        foreach (var category in categories)
        {
            if(category.Count < 1) continue;
            var stall = stallPlacerScript.PlaceNextStall();
            if (stall == null) break;
            stall.GetComponent<StallScript>().SetCategory(category);
        }
    }
}
