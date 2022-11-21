using Assets.Scripts.ClientIO;
using Assets.Scripts.Network;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Shared.DTO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PreLoginScript : MonoBehaviour
{
    [SerializeField]
    GameObject WaitScreenPrefab;

    async void Start()
    {
        var waitScreen = Instantiate(WaitScreenPrefab,new Vector3(),Quaternion.identity);
        waitScreen.SetActive(true);
        //WaitScreenScript waitScreenScript = waitScreen.GetComponent<WaitScreenScript>();
        var canvas = GameObject.Find("Canvas");
        waitScreen.transform.SetParent(canvas.transform,false);
        if (!SessionIO.SessionExists())
        {
            SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
            return;
        }
        var token = SessionIO.LoadSession();
        var req = TokenApi.RefreshToken(token);
        var task = req.SendWebRequest().ToUniTask();
        UnityWebRequest resp = null;
        try
        {
            resp = await task;
        }
        catch (UnityWebRequestException)
        {
            SessionIO.DeleteSession();
            SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
            return;
        }
        if(resp.responseCode == 200) {

            var json = resp.downloadHandler.text;
            token = JsonConvert.DeserializeObject<TokenModelUnity>(json);
            CurrentSession.GetInstance().SetToken(token);
            SessionIO.SaveSession(token);
            SceneManager.LoadScene("MainScene", LoadSceneMode.Single);

            return;
        }
        SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
    }
}
