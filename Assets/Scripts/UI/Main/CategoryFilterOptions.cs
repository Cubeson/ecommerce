using Cysharp.Threading.Tasks;
using Shared.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class CategoryFilterOptions : MonoBehaviour
{
    class FilteredCategory
    {
        public FilteredCategory(CategoryDTO category, Toggle toggle)
        {
            this.toggle = toggle;
            this.category = category;
        }
        private Toggle toggle;
        public bool IsOn() { return toggle.isOn; }
        public CategoryDTO category;
    }


    [SerializeField] GameObject Template;
    [SerializeField] ScrollRect ScrollRect;
    [SerializeField] TMP_InputField CategoryNameFilter;
    [SerializeField] Button ButtonGo;
    [SerializeField] Button ButtonClear;
    [SerializeField] Button ButtonExit;

    async UniTask<CategoryDTO[]> GetCategories()
    {
        UnityWebRequest req = Network.ProductApi.GetCategories();
        UnityWebRequest resp = null;
        IEnumerable<CategoryDTO> categories = null;
        try
        {
            resp = await req.SendWebRequest().ToUniTask();
            categories = JsonConvert.DeserializeObject<CategoryDTO[]>(resp.downloadHandler.text).Where(c => c.Count > 0);
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
        return categories.ToArray();
    }
    CategoryTemplateScript[] allCategories;

    private async UniTask FillContent()
    {
        var categoryDTOs = await GetCategories();
        allCategories = new CategoryTemplateScript[categoryDTOs.Length];
        for (int i = 0; i < categoryDTOs.Length; i++)
        {
            var categoryDTO = categoryDTOs[i];
            var template = Instantiate(Template);
            var templateScript = template.GetComponent<CategoryTemplateScript>();
            templateScript.Setup(categoryDTO);
            allCategories[i] = templateScript;
            template.transform.SetParent(ScrollRect.content);
            template.transform.localScale = Vector3.one;
            template.SetActive(true);
        }
    }

    private void FilterOut(string filter)
    {
        foreach (var script in allCategories)
        {
            if (script.Category.Name.ToUpper().Contains(filter.ToUpper()))
            {
                script.gameObject.SetActive(true);
            }
            else
            {
                script.gameObject.SetActive(false);
            }
        }
    }
    private async void Start()
    {
        Init();
        await FillContent();

    }

    UniTask currentTask;
    public void Init()
    {
        CategoryNameFilter.onValueChanged.AddListener(async (string value) =>
        {
            if (currentTask.Status != UniTaskStatus.Succeeded)
            {
                await currentTask;
            }
            currentTask = UniTask.Create(async () =>
            {
                FilterOut(value);
                await UniTask.Delay(500);
                return UniTask.CompletedTask;
                
            });
            

        });
        ButtonGo.onClick.AddListener(() => {
            
            var categories = allCategories
            .Where(s => s.Toggle.isOn)
            .Select(s => s.Category)
            .ToArray();
            if(categories.Length == 0) return;
            MarketManagerScript.Instance.SetupStalls(categories);
            MenuScript.Instance.PopMenu();

        });
        //ButtonExit.onClick.AddListener(() =>
        //{
        //    MenuScript.Instance.PopMenu();
        //});
        ButtonClear.onClick.AddListener(() =>
        {
            foreach(var script in allCategories) 
            { 
                script.Toggle.isOn = false;
            }
        });
    }
}
