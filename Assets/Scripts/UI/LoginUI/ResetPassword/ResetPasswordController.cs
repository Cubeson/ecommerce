using Assets.Scripts.Extensions;
using Network;
using UnityEngine;
using UnityEngine.UI;
using Shared.DTO;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;

public class ResetPasswordController : MonoBehaviour
{
    [SerializeField] GameObject SendToEmail;
    [SerializeField] GameObject WaitScreenPrefab;
    [SerializeField] InputField InputResetCode;
    [SerializeField] InputField InputPassword1;
    [SerializeField] InputField InputPassword2;
    [SerializeField] Button ButtonSend;
    [SerializeField] Button ButtonBack;
    [SerializeField] Text TextMessage;
    GameObject canvas;
    Color color;
    private void Awake()
    {
        color = TextMessage.color;
    }
    private void OnEnable()
    {
        TextMessage.color = color;
    }
    void Start()
    {
        canvas = GameObject.Find("Canvas");
        ButtonBack.onClick.AddListener(() =>{
            gameObject.SetActive(false);
            SendToEmail.SetActive(true);
        });
        ButtonSend.onClick.AddListener(async () =>
        {
            if (InputResetCode.text.IsNullOrEmpty())
            {
                TextMessage.text = "Invalid Reset Code";
                TextMessage.color = Color.red;
                return;
            }
            if(InputPassword1.text.IsNullOrEmpty())
            {
                TextMessage.text = "Invalid Password";
                TextMessage.color = Color.red;
                return;
            }
            if(!InputPassword1.text.Equals(InputPassword2.text)) 
            {
                TextMessage.text = "Passwords do not match";
                TextMessage.color = Color.red;
                return;
            }

            var req = UserApi.ResetPassword(new ResetPasswordCredentialsUnity() { ResetId = InputResetCode.text, Password = InputPassword1.text });
            var task = req.SendWebRequest().ToUniTask();
            var WaitScreen = Instantiate(WaitScreenPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            WaitScreen.SetActive(true);
            WaitScreen.transform.SetParent(canvas.transform, false);
            WaitScreenScript waitScreenScript = WaitScreen.GetComponent<WaitScreenScript>();
            waitScreenScript.ButtonContinue.onClick.AddListener(() =>
            {
                Destroy(WaitScreen);
            });
            UnityWebRequest resp = null;
            ResetPasswordResponseUnity RPResponse;
            try
            {
                resp = await task;
            }catch(UnityWebRequestException e)
            {
                Debug.Log(e.ToString());
                waitScreenScript.Icon.SetActive(false);
                waitScreenScript.ButtonContinue.gameObject.SetActive(true);
                RPResponse = JsonConvert.DeserializeObject<ResetPasswordResponseUnity>(e.Text);
                waitScreenScript.TextMessage.text = "Error resetting password: " + RPResponse.Message;
                return;
            }
            if(resp.responseCode== 200)
            {
                waitScreenScript.Icon.SetActive(false);
                waitScreenScript.ButtonContinue.gameObject.SetActive(true);
                waitScreenScript.TextMessage.text = "Password changed";
                waitScreenScript.ButtonContinue.onClick.RemoveAllListeners();
                waitScreenScript.ButtonContinue.onClick.AddListener(() =>
                {
                    SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
                });
            }
        });
    }
}
