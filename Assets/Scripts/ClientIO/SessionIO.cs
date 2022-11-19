using Newtonsoft.Json;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.ClientIO
{

    public static class SessionIO
    {
        private static string path = Application.persistentDataPath +"/"+ Constants.SessionFile;
        public static void SaveSession(TokenModelUnity tokenModel)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(tokenModel));

            
#if !UNITY_WEBGL || UNITY_EDITOR

#else

#endif
        }
        public static TokenModelUnity LoadSession()
        {
            return JsonConvert.DeserializeObject<TokenModelUnity>(File.ReadAllText(path));
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
