using Newtonsoft.Json;
using UnityEngine.Networking;
using static Network.NetworkUtility;
using static Constants;
namespace Network
{
	public static class TmpApi
	{
		public static UnityWebRequest GetModelObj()
		{
			var req = UnityWebRequest.Get($"{Url}api/Resources/Tmp1");
			return req;
		}
		public static UnityWebRequest GetModelMtl()
		{
			var req = UnityWebRequest.Get($"{Url}api/Resources/Tmp2");
			return req;
		}
		public static UnityWebRequest GetModel() {
			var req = UnityWebRequest.Get($"{Url}api/Resources/Tmp3");
			return req;
		}
		public static UnityWebRequest GetGLTF()
		{
			var req = UnityWebRequest.Get($"{Url}api/Resources/GLTF");
			return req;
		}
	}
}
