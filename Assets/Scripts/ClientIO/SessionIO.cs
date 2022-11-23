using Newtonsoft.Json;
using Shared.DTO;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.ClientIO
{

    public static class SessionIO
    {
        private static string path = Application.persistentDataPath +"/"+ Constants.SessionFile;
        public static void SaveSession(TokenModelDTOUnity tokenModel)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(tokenModel));

            
#if !UNITY_WEBGL || UNITY_EDITOR

#else

#endif
        }
        public static TokenModelDTOUnity LoadSession()
        {
            return JsonConvert.DeserializeObject<TokenModelDTOUnity>(File.ReadAllText(path));
#if !UNITY_WEBGL || UNITY_EDITOR
            
            //return null;
#else
            //return null;
#endif
        }
        public static bool SessionExists()
        {
            return File.Exists(path);
        }
        public static void DeleteSession()
        {
            File.Delete(path);
        }
    }
}
