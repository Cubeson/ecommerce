using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CASecondViewController : MonoBehaviour
{
    [SerializeField] InputField InputFirstName;
    [SerializeField] InputField InputLastName;
    [SerializeField] Button ButtonNext;
    CAViewFunctions viewFunctions;
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
    }
}
