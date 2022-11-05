using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginViewController : MonoBehaviour
{
    [SerializeField] GameObject CreateAccountView;
    [SerializeField] Button CreateAccountButton;
    [SerializeField] Button ForgotPasswordButton;
    void Start()
    {
        CreateAccountButton.onClick.AddListener(() =>
        {
            CreateAccountView.SetActive(true);
            gameObject.SetActive(false);
        });
    }
}
