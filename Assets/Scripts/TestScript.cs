using Cysharp.Threading.Tasks;
using System.IO;
using UnityEngine;
using Siccity.GLTFUtility;
using System.Collections.Generic;

public class TestScript : MonoBehaviour
{
    [SerializeField]
    GameObject ModelView;
	async void Start()
    {
        var controller = ModelView.GetComponent<ModelViewControllerScript>();

        var resp = await Network.TmpApi.GetGLTF().SendWebRequest().ToUniTask();
     
        controller.SetModel(resp.downloadHandler.data);

    }


	async UniTask<byte[]> MyTask()
	{

		//var reqModel = Network.ProductApi.GetModel(12);
		var reqModel = Network.TmpApi.GetGLTF();
        var respModel = await reqModel.SendWebRequest().ToUniTask();
		return respModel.downloadHandler.data;

        //var GO = Importer.LoadFromBytes(await MyTask());
        //await UniTask.Delay(1000);
    }

}
