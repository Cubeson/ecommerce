namespace Server
{
    public class JWTSecretKey
    {
        public readonly string Key;
        public readonly string Issuer;
        public readonly string Audience;
        private static JWTSecretKey? instance;
        private JWTSecretKey(string key, string issuer, string audience)
        {
            Key = key;
            Issuer = issuer;
            Audience = audience;
        }
        public static void Register(string key,string issuer,string audience)
        {
            if(instance != null) throw new InvalidOperationException("Already registered");
            instance = new JWTSecretKey(key,issuer,audience);
        }
        public static JWTSecretKey Get() {
            if(instance == null) throw new InvalidOperationException("Not registered yet");
            return instance;
        }
    }
}
