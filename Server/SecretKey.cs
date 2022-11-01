namespace Server
{
    public class SecretKey
    {
        public readonly string Key;
        public readonly string Issuer;
        public readonly string Audience;
        private static SecretKey? secret;
        private SecretKey(string key, string issuer, string audience)
        {
            Key = key;
            Issuer = issuer;
            Audience = audience;
        }
        public static void RegisterSecret(string key,string issuer,string audience)
        {
            if(secret != null) throw new InvalidOperationException("Already registered");
            secret = new SecretKey(key,issuer,audience);
        }
        public static SecretKey GetSecret() {
            if(secret == null) throw new InvalidOperationException("Not registered yet");
            return secret;
        }
    }
}
