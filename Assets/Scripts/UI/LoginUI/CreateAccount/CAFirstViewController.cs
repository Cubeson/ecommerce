using UnityEngine;
using UnityEngine.UI;
public class CAFirstViewController : MonoBehaviour
{
    
    [SerializeField] Button ButtonNext;
    [SerializeField] InputField InputEmail;
    [SerializeField] Text TextMessage;
    CAViewFunctions viewFunctions;
    Color color;
    public void Start()
    {
        var parent = transform.parent.gameObject;
        var controller = parent.GetComponent<CAController>();
        viewFunctions = GetComponent<CAViewFunctions>();
        viewFunctions.allowNext = () => Validators.ValidateEmail(InputEmail.text);
        viewFunctions.saveData = () => { CreateAccoundCredentials.Email = TextMessage.text; };
        ButtonNext.onClick.AddListener(() =>
        {
            if(!Validators.ValidateEmail(InputEmail.text))
            {
                TextMessage.text = "Invalid email address";
                TextMessage.color = Color.red;
            }
            else
            {
                controller.Next();
            }
        });

    }
    private void Awake()
    {
        color = TextMessage.color;
    }
    private void OnEnable()
    {
        TextMessage.text = "Type in your email address";
        TextMessage.color = color;
    }
}
