using Cysharp.Threading.Tasks;
using Network;
using Shared.DTO;
using Newtonsoft.Json;
using System.Collections.Generic;
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
        UserCreateDTOUnity user = new UserCreateDTOUnity()
        {
            Email = CreateAccoundCredentials.Email,
            FirstName = CreateAccoundCredentials.FirstName,
            LastName = CreateAccoundCredentials.LastName,
            Password = CreateAccoundCredentials.Password,
        };
        var req = UserEndpoint.CreateUser(user);
        //var task = UniTask.Create(() =>
        //{
        //    return req.SendWebRequest().ToUniTask();
        //});
        var task = req.SendWebRequest().ToUniTask();
        WaitScreen = Instantiate(WaitScreenPrefab,new Vector3(0,0,0),Quaternion.identity);
        WaitScreen.SetActive(true);
        WaitScreen.transform.SetParent(canvas.transform,false);
        CAWaitScreen waitScreenScript = WaitScreen.GetComponent<CAWaitScreen>();
        UnityWebRequest resp = null;
        CreateAccountResponseUnity CAResponse = null;
        try
        {
            resp = await task;
        }catch(UnityWebRequestException e)
        {
            waitScreenScript.Icon.SetActive(false);
            waitScreenScript.ButtonContinue.gameObject.SetActive(true);
            CAResponse = JsonConvert.DeserializeObject<CreateAccountResponseUnity>(e.Text);
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
}
