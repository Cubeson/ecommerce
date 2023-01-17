using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Shared.SortOrderDB;
using TMPro;
using static TMPro.TMP_Dropdown;
using System.Linq;

public class ProductFilterOptionsScript : MonoBehaviour
{
    [SerializeField] GameObject ButtonExit;
    [SerializeField] GameObject ButtonSearch;
    [SerializeField] GameObject DropdownSort;
    [SerializeField] GameObject ProductName;
    [SerializeField] GameObject PriceMax;
    [SerializeField] GameObject PriceMin;

    TMP_Dropdown dropdown;
    TMP_InputField productNameInput;
    TMP_InputField priceMaxInput;
    TMP_InputField priceMinInput;

    public static ProductFilterOptionsScript instance;
    public static ProductFilterOptionsScript Instance { get { return instance; } }
    private StallScript _stallScript;
    public void OpenMenu(StallScript stallScript)
    {
        if(_stallScript != stallScript)
        {
            _stallScript= stallScript;
            ResetUI();
        }
        MenuScript.Instance.PushMenu(gameObject);

    }
    private void ResetUI()
    {

    }

    struct OptionWrapper
    {
        public SortOrderDB SortOrder { get; }
        public string Name { get; }
        public OptionWrapper(SortOrderDB sortOrderDB, string name)
        {
            SortOrder = sortOrderDB;
            Name = name;
        }
    }

    OptionWrapper[] optionsWrapper =
    {
        new(SortOrderDB.DateModified_Desc,"Newest"),
        new(SortOrderDB.DateModified_Asc,"Oldest"),
        new(SortOrderDB.Price_Asc,"Cheapest"),
        new(SortOrderDB.Price_Desc,"Costiest"),

    };
    public void Init()
    {
        instance = this;

        dropdown = DropdownSort.GetComponent<TMP_Dropdown>();
        productNameInput = ProductName.GetComponent<TMP_InputField>();
        priceMaxInput = PriceMax.GetComponent<TMP_InputField>();
        priceMinInput = PriceMin.GetComponent<TMP_InputField>();

        OptionData x = new OptionData(SortOrderDB.Price_Asc.ToString());
        dropdown.options = optionsWrapper.Select(o => new OptionData(o.Name)).ToList();

        //ButtonExit.GetComponent<Button>().onClick.AddListener( () =>
        //{
        //    MenuScript.Instance.PopMenu();
        //} );
        ButtonSearch.GetComponent<Button>().onClick.AddListener(() =>
        {
            decimal min = 0, max = 0;
            decimal.TryParse(priceMinInput.text, out min);
            decimal.TryParse(priceMaxInput.text, out max);
            
            var index = dropdown.value;
            var sortOrder = optionsWrapper[index].SortOrder;
            var name = productNameInput.text;
            _stallScript.SetFilter(sortOrder, name, min, max);
            _stallScript.GetProductsFromServer();
            MenuScript.Instance.PopMenu();
        });
    }
}
