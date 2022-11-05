namespace Server
{
    public class SmtpCredentials
    {
        public readonly string Email;
        public readonly string Password;
        private static SmtpCredentials? instance;
        private SmtpCredentials(string email, string password)
        {
            Email = email;
            Password = password;
        }
        public static void Register(string email,string password)
        {
            if (instance != null) throw new InvalidOperationException("Already registered");
            instance = new SmtpCredentials(email,password);
        }
        public static SmtpCredentials Get()
        {
            if (instance == null) throw new InvalidOperationException("Not registered yet");
            return instance;
        }
    }
}
