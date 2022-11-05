using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CAThirdViewController : MonoBehaviour
{
    [SerializeField] Button ButtonNext;
    [SerializeField] InputField InputPassword1;
    [SerializeField] InputField InputPassword2;
    [SerializeField] Text TextMessage;
    CAViewFunctions viewFunctions;

    void Start()
    {
        viewFunctions = GetComponent<CAViewFunctions>();
        viewFunctions.allowNext = () => { 
            return Validators.IsValidPassword(InputPassword1.text) && InputPassword1.text.Equals(InputPassword2.text);
        };
        viewFunctions.saveData = () => {
            CreateAccoundCredentials.Password = InputPassword1.text;
        };
    }


}
