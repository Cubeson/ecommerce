using Cysharp.Threading.Tasks;
using Network;
using Newtonsoft.Json;
using Shared.DTO;
using System;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Network
{
    public sealed class CurrentSession
    {
        private CurrentSession() { }
        private static CurrentSession _instance;
        private DateTime expires;

        private TokenModelDTO _tokenModel = null;
        public static CurrentSession Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CurrentSession();
                }
                return _instance;
            }
        }
        public async UniTask<TokenModelDTO> GetToken()
        {
            if(DateTime.Now > expires)
            {
                var req = TokenApi.RefreshToken(_tokenModel);
                UnityWebRequest resp = null;
                try
                {
                    resp = await req.SendWebRequest().ToUniTask();
                }catch(UnityWebRequestException)
                {
                    SceneManager.LoadScene("LoginScene", LoadSceneMode.Single);
                    _tokenModel= null;
                    expires = DateTime.MinValue;
                    return null;
                }
                var token = JsonConvert.DeserializeObject<TokenModelDTO>(resp.downloadHandler.text);
                SetToken(token);
            }
            return _tokenModel;
        }
        public void SetToken(TokenModelDTO tokenModel)
        {
            _tokenModel = tokenModel;
            expires = DateTime.Now.AddSeconds(_tokenModel.ExpiresInSeconds);
        }

    }
}
