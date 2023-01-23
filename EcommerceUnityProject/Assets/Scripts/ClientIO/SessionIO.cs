﻿using Newtonsoft.Json;
using Shared.DTO;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.ClientIO
{

    public static class SessionIO
    {
        private static string path = Application.persistentDataPath +"/"+ Constants.SessionFile;
        public static void SaveSession(TokenModelDTO tokenModel)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(tokenModel));
        }
        public static TokenModelDTO LoadSession()
        {
            return JsonConvert.DeserializeObject<TokenModelDTO>(File.ReadAllText(path));
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