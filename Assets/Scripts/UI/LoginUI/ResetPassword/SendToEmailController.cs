using Network;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Shared.DTO;
using Newtonsoft.Json;
using Shared.Validators;

public sealed class MyObject{
    public string Cool;
}
public class SendToEmailController : MonoBehaviour
{
    [SerializeField] GameObject ResetPasswordView;
    [SerializeField] Button BackButton;
    [SerializeField] Button ButtonNext;
    [SerializeField] Button ButtonSendToEmail;
    [SerializeField] InputField InputEmail;
    [SerializeField] Text TextSendEmail;
    [SerializeField] GameObject BlackPanel;
    Color color;
    private void OnEnable()
    {
        TextSendEmail.text = 
@"Request a password reset.
If an account with this email exists,
a Reset Code will be sent to this email";
        TextSendEmail.color = color;
    }
    private void Awake()
    {
        color = TextSendEmail.color;
    }
    void Start()
    {
        var json = JsonConvert.SerializeObject(new UserLoginDTOUnity() { Email="b",Password="a"});
        BackButton.onClick.AddListener(() => {
            SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
        });
        ButtonNext.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            ResetPasswordView.SetActive(true);
        });
        ButtonSendToEmail.onClick.AddListener(() =>
        {
            if (!Validators.isValidEmail(InputEmail.text))
            {
                TextSendEmail.text = "Invalid email address";
                TextSendEmail.color = Color.red;
                return;
            }
            var req = UserApi.RequestResetPasswordCode(new RequestResetPasswordUnity() { Email= InputEmail.text });
            req.SendWebRequest();
            BlackPanel.SetActive(true);
        });
    }
}
