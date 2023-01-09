using Assets.Scripts.ClientIO;
using Assets.Scripts.Network;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Shared.DTO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Network;
public class PreLoginScript : MonoBehaviour
{
    [SerializeField]
    GameObject WaitScreenPrefab;

    async void Start()
    {
        var waitScreen = Instantiate(WaitScreenPrefab,new Vector3(),Quaternion.identity);
        waitScreen.SetActive(true);
        var canvas = GameObject.Find("Canvas");
        waitScreen.transform.SetParent(canvas.transform,false);
        if (!SessionIO.SessionExists())
        {
            SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
            return;
        }
        var token = SessionIO.LoadSession();
        var req = TokenApi.RefreshToken(token);
        UnityWebRequest resp = null;
        try
        {
            resp = await req.SendWebRequest().ToUniTask();
            var json = resp.downloadHandler.text;
            token = JsonConvert.DeserializeObject<TokenModelDTO>(json);
        }
        catch (UnityWebRequestException)
        {
            SessionIO.DeleteSession();
            SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
            return;
        }
        finally
        {
            resp?.Dispose();
            req?.Dispose();
        }

        CurrentSession.Instance.SetToken(token);
        SessionIO.SaveSession(token);
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);


    }
}
