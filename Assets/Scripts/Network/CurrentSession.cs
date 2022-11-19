using Shared.DTO;

namespace Assets.Scripts.Network
{
    public sealed class CurrentSession
    {
        private CurrentSession() { }
        private static CurrentSession _instance;

        private TokenModelUnity _tokenModel = null;
        public static CurrentSession GetInstance()
        {
            if(_instance == null)
            {
                _instance = new CurrentSession();
            }
            return _instance;
        }
        public TokenModelUnity GetToken()
        {
            return _tokenModel;
        }
        public void SetToken(TokenModelUnity tokenModel)
        {
            _tokenModel = tokenModel;
        }

    }
}
