using Shared.DTO;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryTemplateScript : MonoBehaviour
{
    [SerializeField] GameObject TextGo;
    public Toggle Toggle;
    public CategoryDTO Category;

    public void Setup(CategoryDTO category)
    {
        Category = category;
        TextGo.GetComponent<TMP_Text>().text = category.Name;
    }
    

}
