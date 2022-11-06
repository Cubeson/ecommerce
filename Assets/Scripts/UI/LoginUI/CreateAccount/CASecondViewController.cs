using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CASecondViewController : MonoBehaviour
{
    [SerializeField] InputField InputFirstName;
    [SerializeField] InputField InputLastName;
    [SerializeField] Button ButtonNext;
    [SerializeField] Text TextMessage;
    CAViewFunctions viewFunctions;
    Color color;
    bool IsFirstNameOk()
    {
        string name = InputFirstName.text;
        if (name == null || name.Equals("") )return false;
        return true;
    }
    bool IsLastNameOk()
    {
        string name = InputLastName.text;
        if (name == null || name.Equals("")) return false;
        return true;
    }
    void Start()
    {
        color = TextMessage.color;
        viewFunctions = GetComponent<CAViewFunctions>();
        viewFunctions.allowNext = () =>
        {
            return IsFirstNameOk() && IsLastNameOk();
        };
        viewFunctions.saveData = () => 
        {
            CreateAccoundCredentials.FirstName = InputFirstName.text;
            CreateAccoundCredentials.LastName = InputLastName.text;
        };
        ButtonNext.onClick.AddListener(() =>
        {
            if (!gameObject.activeSelf) return;
            if(!IsFirstNameOk() || !IsLastNameOk())
            {
                TextMessage.text = "First name or last name are invalid";
                TextMessage.color = Color.red;
            }
        });
    }
    //int i = 0;
    private void OnEnable()
    {
        //Debug.Log("Enable" + i++);
        TextMessage.text = "Type in your first and last name";
        TextMessage.color = color;
    }
    private void Awake()
    {
        color = TextMessage.color;
    }
}
