using Cysharp.Threading.Tasks;
using Shared.Validators;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
public class CAThirdViewController : MonoBehaviour
{
    [SerializeField] Button ButtonNext;
    [SerializeField] InputField InputPassword1;
    [SerializeField] InputField InputPassword2;
    [SerializeField] Text TextMessage;
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
    }
    private void OnEnable()
    {
        TextMessage.color = color;
        TextMessage.text = "Type in your password and repeat it";

    }
    private void Awake()
    {
        color = TextMessage.color;
    }

}
