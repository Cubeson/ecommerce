using Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClick : MonoBehaviour
{
    void Start()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(fn);
    }
    void fn() {
        var request = UnityWebRequest.Get(ServerUrl.Url + "HelloWorld");
        var op =request.SendWebRequest();
        while (!op.isDone) { }
        Debug.Log(op.webRequest.downloadHandler.text);

    }
}
