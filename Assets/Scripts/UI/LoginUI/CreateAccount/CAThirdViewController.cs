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
    Color color;
    void Start()
    {
        color = TextMessage.color;
        ButtonNext.onClick.AddListener(() =>
        {
            if (!gameObject.activeSelf) return;
            if(!Validators.IsValidPassword(InputPassword1.text) || !InputPassword1.text.Equals(InputPassword2.text))
            {
                TextMessage.text = "Passwords are invalid or not identical";
                TextMessage.color = Color.red;
            } 
            
        });
        viewFunctions = GetComponent<CAViewFunctions>();
        viewFunctions.allowNext = () => { 
            return Validators.IsValidPassword(InputPassword1.text) && InputPassword1.text.Equals(InputPassword2.text);
        };
        viewFunctions.saveData = () => {
            CreateAccoundCredentials.Password = InputPassword1.text;
        };
    }
    private void OnEnable()
    {
        TextMessage.color = color;
        TextMessage.text = "Type in your password and repeat it";
    }
    private void Awake()
    {
        color = TextMessage.color;
    }


}
