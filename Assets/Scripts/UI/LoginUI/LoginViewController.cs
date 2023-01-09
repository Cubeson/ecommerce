using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Shared.Validators;
using Shared.DTO;
using Network;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using Assets.Scripts.Network;
using Assets.Scripts.ClientIO;
using Newtonsoft.Json;

public class LoginViewController : MonoBehaviour
{
    [SerializeField] Button CreateAccountButton;
    [SerializeField] Button ForgotPasswordButton;
    [SerializeField] InputField InputEmail;
    [SerializeField] InputField InputPassword;
    [SerializeField] Button ButtonLogin;
    [SerializeField] GameObject WaitScreenPrefab;
    GameObject canvas;
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        CreateAccountButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("CreateAccountScene", LoadSceneMode.Single);
        });
        ButtonLogin.onClick.AddListener(async () =>
        {
            if (!Validators.isValidEmail(InputEmail.text) || !Validators.IsValidPassword(InputPassword.text)) return;
            var req = UserApi.LoginUser(new UserLoginDTO() {Email = InputEmail.text, Password = InputPassword.text });
            var task = req.SendWebRequest().ToUniTask();
            var waitScreen = Instantiate(WaitScreenPrefab, new Vector3(), Quaternion.identity);
            waitScreen.SetActive(true);
            waitScreen.transform.SetParent(canvas.transform, false);
            WaitScreenScript waitScreenScript = waitScreen.GetComponent<WaitScreenScript>();
            UnityWebRequest resp = null;
            waitScreenScript.ButtonContinue.onClick.AddListener(() =>
            {
                Destroy(waitScreen);
            });
            try
            {
                resp = await task;
            }
            catch (UnityWebRequestException)
            {
                waitScreenScript.Icon.SetActive(true);
                waitScreenScript.ButtonContinue.gameObject.SetActive(true);
                waitScreenScript.TextMessage.text = "Provided login credentials are incorrect";
                return;
            } 
            if(resp.responseCode == 200)
            {
                var json = resp.downloadHandler.text;
                TokenModelDTO tm = JsonConvert.DeserializeObject<TokenModelDTO>(json);
                CurrentSession.Instance.SetToken(tm);
                SessionIO.SaveSession(tm);
                waitScreenScript.Icon.SetActive(false);
                waitScreenScript.ButtonContinue.gameObject.SetActive(true);
                waitScreenScript.TextMessage.text = "Logged in";
                waitScreenScript.ButtonContinue.onClick.AddListener(() =>
                {
                    SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
                });
            }


        });
        ForgotPasswordButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("ResetPasswordScene", LoadSceneMode.Single);
        });
    }
}
