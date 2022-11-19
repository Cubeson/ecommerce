using Assets.Scripts.Network;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[RequireComponent(typeof(Button))]
public class LogoutScript : MonoBehaviour
{
    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            var req = TokenApi.RevokeToken(CurrentSession.GetInstance().GetToken());

            req.SendWebRequest();
            SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
        });
    }
}
