using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoginViewController : MonoBehaviour
{
    [SerializeField] Button CreateAccountButton;
    [SerializeField] Button ForgotPasswordButton;
    [SerializeField] InputField InputEmail;
    [SerializeField] InputField InputPassword;
    [SerializeField] Button ButtonLogin;
    void Start()
    {
        CreateAccountButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("CreateAccountScene", LoadSceneMode.Single);
        });
        ButtonLogin.onClick.AddListener(() =>
        {

        });
        ForgotPasswordButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("ResetPasswordScene", LoadSceneMode.Single);
        });
    }
}
