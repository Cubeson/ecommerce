using Newtonsoft.Json;
using UnityEngine.Networking;
using static Network.NetworkUtility;
using static Constants;
namespace Network
{
	public static class TmpApi
	{
		public static UnityWebRequest TMP()
		{
			var req = UnityWebRequest.Get($"{Url}api/TMP");
			return req;
		}

	}
}
