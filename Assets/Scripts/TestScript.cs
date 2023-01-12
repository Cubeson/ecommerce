using Cysharp.Threading.Tasks;
using System.IO;
using UnityEngine;
//using Siccity.GLTFUtility;
using System.Collections.Generic;

public class TestScript : MonoBehaviour
{
    [SerializeField]
    GameObject ModelView;
	async void Start()
    {

        var resp = await Network.TmpApi.TMP().SendWebRequest().ToUniTask();
        var txt = resp.downloadHandler.text;
        Debug.Log(txt);
        //var controller = ModelView.GetComponent<ModelViewControllerScript>();
        //
        //var resp = await Network.TmpApi.GetGLTF().SendWebRequest().ToUniTask();
        //
        //controller.SetModel(resp.downloadHandler.data);

    }

}
