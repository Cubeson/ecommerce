using UnityEngine.Networking;
using static UnityEngine.Networking.UnityWebRequest;
using Newtonsoft.Json;
using Network.DTO;
using Debug = UnityEngine.Debug;
namespace Network
{
    internal static class UserEndpoint
    {
        public static UnityWebRequestAsyncOperation CreateUser(UserCreateDTO userDTO)
        {
            var json = JsonConvert.SerializeObject(userDTO);
            Debug.Log(json);
            /*
             * UnityWebRequest.Post somehow breaks json body 
             * using Put and changing method to Post bypasses this issue
             */
            var request = Put(ServerUrl.Url + "User/Create/", json);
            request.method = "Post";
            
            request.SetRequestHeader("Content-Type", "application/json");
            Debug.Log(request.url);
            var operation = request.SendWebRequest();
            Debug.Log(request.responseCode);
            return operation;
            
        }
    }
}
