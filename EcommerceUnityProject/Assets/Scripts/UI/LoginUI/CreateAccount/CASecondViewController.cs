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
        var parent = transform.parent.gameObject;
        var controller = parent.GetComponent<CAController>();
        ButtonNext.onClick.AddListener(() =>
        {
            if(!IsFirstNameOk() || !IsLastNameOk())
            {
                TextMessage.text = "First name or last name are invalid";
                TextMessage.color = Color.red;
            }
            controller.Next();
        });
    }
    private void OnEnable()
    {
        TextMessage.text = "Type in your first and last name";
        TextMessage.color = color;
    }
    private void Awake()
    {
        color = TextMessage.color;
    }
}
