using Cysharp.Threading.Tasks;
using Network;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class CAThirdViewController : MonoBehaviour
{
    [SerializeField] Button ButtonNext;
    [SerializeField] InputField InputPassword1;
    [SerializeField] InputField InputPassword2;
    [SerializeField] Text TextMessage;
    CAViewFunctions viewFunctions;
    Color color;
    void Start()
    {
        color = TextMessage.color;
        var parent = transform.parent.gameObject;
        var controller = parent.GetComponent<CAController>();
        ButtonNext.onClick.AddListener(() =>
        {
            if(!Validators.IsValidPassword(InputPassword1.text) || !InputPassword1.text.Equals(InputPassword2.text))
            {
                TextMessage.text = "Passwords are invalid or not identical";
                TextMessage.color = Color.red;
            }
            else
            {
                controller.Next();
            }
            
        });
        viewFunctions = GetComponent<CAViewFunctions>();
        viewFunctions.allowNext = () => { 
            return Validators.IsValidPassword(InputPassword1.text) && InputPassword1.text.Equals(InputPassword2.text);
        };
        viewFunctions.saveData = () => {
            CreateAccoundCredentials.Password = InputPassword1.text;
        };
    }
    private void OnEnable()
    {
        TextMessage.color = color;
        TextMessage.text = "Type in your password and repeat it";
        //UnityWebRequest request = new UnityWebRequest()
        //{
        //    url = ServerUrl.Url + "Hello",
        //    method = "GET",
        //    timeout = 5,
        //    downloadHandler = new DownloadHandlerBuffer()
        //
        //};
        //UniTask.Create(async () => { Debug.Log(await Str(request)); });

    }
    private void Awake()
    {
        color = TextMessage.color;
    }
    async UniTask<string> Str(UnityWebRequest request)
    {
        var resp = await request.SendWebRequest();
        return resp.downloadHandler.text;
    }


}
