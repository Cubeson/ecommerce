using Shared.DTO;

namespace Assets.Scripts.Network
{
    public sealed class CurrentSession
    {
        private CurrentSession() { }
        private static CurrentSession _instance;

        private TokenModelDTOUnity _tokenModel = null;
        public static CurrentSession GetInstance()
        {
            if(_instance == null)
            {
                _instance = new CurrentSession();
            }
            return _instance;
        }
        public TokenModelDTOUnity GetToken()
        {
            return _tokenModel;
        }
        public void SetToken(TokenModelDTOUnity tokenModel)
        {
            _tokenModel = tokenModel;
        }

    }
}
