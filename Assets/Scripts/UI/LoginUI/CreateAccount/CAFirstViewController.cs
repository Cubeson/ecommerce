
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class CAFirstViewController : MonoBehaviour
{
    [SerializeField] Button ButtonNext;
    [SerializeField] InputField InputEmail;
    [SerializeField] Text TextMessage;
    CAViewFunctions viewFunctions;
    public void Start()
    {
        viewFunctions = GetComponent<CAViewFunctions>();
        viewFunctions.allowNext = () => Validators.ValidateEmail(InputEmail.text);
        viewFunctions.saveData = () => { CreateAccoundCredentials.Email = TextMessage.text; };
        ButtonNext.onClick.AddListener(() =>
        {
            if(!Validators.ValidateEmail(InputEmail.text))
            {
                TextMessage.text = "Invalid email address";
                TextMessage.color = Color.red;
            }
        });

    }
}
