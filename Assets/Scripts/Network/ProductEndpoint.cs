using static UnityEngine.Networking.UnityWebRequest;
using Debug = UnityEngine.Debug;
namespace Network
{
    internal static class ProductEndpoint
    {

        public static byte[] DownloadProduct(int id)
        {
            var op = Get(Server.Url + "Download/id/1").SendWebRequest();
            while (!op.isDone) { }
            return op.webRequest.downloadHandler.data;
        }
    }
}
