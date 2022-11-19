using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.ClientIO
{
    public static class SessionIO
    {
        public static void SaveSession()
        {
#if !UNITY_WEBGL || UNITY_EDITOR

#endif
        }
        public static TokenModel LoadSession()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            
            return null;
#endif
            return null;
        }
    }
}
