using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoginViewController : MonoBehaviour
{
    [SerializeField] Button CreateAccountButton;
    [SerializeField] Button ForgotPasswordButton;
    void Start()
    {
        CreateAccountButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("CreateAccountScene", LoadSceneMode.Single);
        });
    }
}
