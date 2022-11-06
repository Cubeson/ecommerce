
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
    Color color;
    public void Start()
    {
        viewFunctions = GetComponent<CAViewFunctions>();
        viewFunctions.allowNext = () => Validators.ValidateEmail(InputEmail.text);
        viewFunctions.saveData = () => { CreateAccoundCredentials.Email = TextMessage.text; };
        ButtonNext.onClick.AddListener(() =>
        {
            //Guid guid= Guid.NewGuid();
            //Debug.Log(guid.ToString() + gameObject.GetInstanceID());
            if (!gameObject.activeSelf) return;
            if(!Validators.ValidateEmail(InputEmail.text))
            {
                //Debug.Log(guid.ToString() + gameObject.GetInstanceID());
                
                TextMessage.text = "Invalid email address";
                TextMessage.color = Color.red;
            }
        });

    }
    private void Awake()
    {
        color = TextMessage.color;
    }
    private void OnEnable()
    {
        TextMessage.text = "Type in your email address";
        TextMessage.color = color;
    }
}
