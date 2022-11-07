using Assets.Scripts.Network.DTO;
using Cysharp.Threading.Tasks;
using Network;
using Network.DTO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Shared;
public class CAController : MonoBehaviour
{
    [SerializeField] List<GameObject> ViewList;
    [SerializeField] Button ButtonBack;
    [SerializeField] GameObject WaitScreenPrefab;
    GameObject canvas;
    GameObject WaitScreen;
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
    public void Next()
    {
        CASaveData saveData = ViewList[index].GetComponent<CASaveData>();
        saveData.saveData();
        if (index < ViewList.Count - 1) {

            ViewList[index].SetActive(false);
            index++;
            ViewList[index].SetActive(true);

        }
        else
        {
            UniTask.Create(() => CreateAccount());
        }
    }

    private async UniTask CreateAccount()
    {
        UserCreateDTO user = new UserCreateDTO()
        {
            Email = CreateAccoundCredentials.Email,
            FirstName = CreateAccoundCredentials.FirstName,
            LastName = CreateAccoundCredentials.LastName,
            Password = CreateAccoundCredentials.Password,
        };
        var json = JsonConvert.SerializeObject(user);
        var req = new UnityWebRequest()
        {
            method = "POST",
            url = ServerUrl.Url + "api/User/Create",
            downloadHandler = new DownloadHandlerBuffer(),
            uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(json)),
            timeout = 8,
        };
        req.SetRequestHeader("Content-Type", "application/json");
        var task = UniTask.Create(() =>
        {
            return req.SendWebRequest().ToUniTask();
        });
        WaitScreen = Instantiate(WaitScreenPrefab,new Vector3(0,0,0),Quaternion.identity);
        WaitScreen.SetActive(true);
        //WaitScreen.transform.parent = canvas.transform;
        WaitScreen.transform.SetParent(canvas.transform,false);
        CAWaitScreen waitScreenScript = WaitScreen.GetComponent<CAWaitScreen>();
        UnityWebRequest resp = null;
        CreateAccountResponse CAResponse = null;
        try
        {
            resp = await task;
        }catch(UnityWebRequestException e)
        {
            waitScreenScript.Icon.SetActive(false);
            waitScreenScript.ButtonContinue.gameObject.SetActive(true);
            CAResponse = JsonConvert.DeserializeObject<CreateAccountResponse>(e.Text);
            waitScreenScript.TextMessage.text = "Error creating an account: " + CAResponse.Message;
            waitScreenScript.ButtonContinue.onClick.AddListener(() =>
            {
                Destroy(WaitScreen);
            });
            return;
        }

        if (resp.responseCode == 200)
        {
            waitScreenScript.Icon.SetActive(false);
            waitScreenScript.ButtonContinue.gameObject.SetActive(true);
            waitScreenScript.TextMessage.text = "Created an accont";
            waitScreenScript.ButtonContinue.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("LoginScene",LoadSceneMode.Single);
            });
        }      
    }

    void Start()
    {
        if (ViewList.Count == 0) {
            throw new MissingReferenceException("CAController ViewList needs items added in editor");
        }
        canvas = GameObject.Find("Canvas");
        ButtonBack.onClick.AddListener(Back);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
