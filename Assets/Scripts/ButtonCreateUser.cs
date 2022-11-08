using Network;
using UnityEngine;
using UnityEngine.UI;
using Shared.DTO;

[RequireComponent(typeof(Button))]
public class ButtonCreateUser : MonoBehaviour
{
    [SerializeField]private InputField FirstName;
    [SerializeField]private InputField LastName;
    [SerializeField]private InputField Email;
    [SerializeField]private InputField Password;
    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(ButtonEvent);
    }

    private void ButtonEvent()
    {
        var firstName = FirstName.text;
        var lastName = LastName.text;
        var email = Email.text;
        var password = Password.text;

        var operation = UserEndpoint.CreateUser(new UserCreateDTOUnity()
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Password = password
        });
        while (!operation.isDone) { }
        //Debug.Log(operation.webRequest.downloadHandler.text);
    }
}
