using Cysharp.Threading.Tasks;
using System.IO;
using UnityEngine;
using Siccity.GLTFUtility;
public class TestScript : MonoBehaviour
{

	async void Start()
    {
            var GO = Importer.LoadFromBytes(await MyTask());
			await UniTask.Delay(1000);
	}


	async UniTask<byte[]> MyTask()
	{
        var reqModel = Network.ProductApi.GetModel(12);
        var respModel = await reqModel.SendWebRequest().ToUniTask();
		return respModel.downloadHandler.data;
    }

}
