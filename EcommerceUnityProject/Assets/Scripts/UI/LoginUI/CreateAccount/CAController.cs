using Cysharp.Threading.Tasks;
using Network;
using Shared.DTO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Shared;
using Newtonsoft.Json;

public class CAController : MonoBehaviour
{
    [SerializeField] List<GameObject> ViewList;
    [SerializeField] Button ButtonBack;
    [SerializeField] GameObject WaitScreenPrefab;
    [SerializeField] InputField InputFirstName;
    [SerializeField] InputField InputLastName;
    [SerializeField] InputField InputEmail;
    [SerializeField] InputField InputPassword;
    GameObject canvas;
    //GameObject WaitScreen;
    int index = 0;
    public void Back()
    {
        if (index > 0) {
            ViewList[index].SetActive(false);
            index--;
            ViewList[index].SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
        }
    }
    public async void Next()
    {
        if (index < ViewList.Count - 1) {

            ViewList[index].SetActive(false);
            index++;
            ViewList[index].SetActive(true);

        }
        else
        {
            //UniTask.Create(() => CreateAccount());
            await CreateAccount();
        }
    }

    private async UniTask CreateAccount()
    {
        UserCreateDTO user = new()
        {
            Email = InputEmail.text,
            FirstName = InputFirstName.text,
            LastName = InputLastName.text,
            Password = InputPassword.text,
        };
        var req = UserApi.CreateUser(user);
        var task = req.SendWebRequest().ToUniTask();
        var waitScreen = Instantiate(WaitScreenPrefab,new Vector3(0,0,0),Quaternion.identity);
        waitScreen.SetActive(true);
        waitScreen.transform.SetParent(canvas.transform,false);
        WaitScreenScript waitScreenScript = waitScreen.GetComponent<WaitScreenScript>();
        UnityWebRequest resp = null;
        GenericResponseDTO CAResponse = null;
        waitScreenScript.ButtonContinue.onClick.AddListener(() =>
        {
            Destroy(waitScreen);
        });
        try
        {
            resp = await task;
        }catch(UnityWebRequestException e)
        {
            waitScreenScript.Icon.SetActive(false);
            waitScreenScript.ButtonContinue.gameObject.SetActive(true);
            CAResponse = JsonConvert.DeserializeObject<GenericResponseDTO>(e.Text);
            waitScreenScript.TextMessage.text = "Error creating an account: " + CAResponse.Message;
            return;
        }
        finally
        {
            req?.Dispose();
            resp?.Dispose();
        }

        waitScreenScript.Icon.SetActive(false);
        waitScreenScript.ButtonContinue.gameObject.SetActive(true);
        waitScreenScript.TextMessage.text = "Created an accont";
        waitScreenScript.ButtonContinue.onClick.RemoveAllListeners();
        waitScreenScript.ButtonContinue.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("LoginScene",LoadSceneMode.Single);
        });     
    }

    void Start()
    {
        if (ViewList.Count == 0) {
            throw new MissingReferenceException("CAController ViewList needs elements added in editor");
        }
        canvas = GameObject.Find("Canvas");
        ButtonBack.onClick.AddListener(Back);
    }
}