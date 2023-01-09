namespace Server.Utility
{
    public static class Constants
    {
        public static readonly int TOKEN_EXPIRATION_TIME_MINUTES = 30;
        public static readonly int REFRESH_TOKEN_EXPIRATION_TIME_HOURS = 6;

        public static readonly int PASSWORD_RESET_LIFETIME_MINUTES = 15;

        public static readonly string ROLE_DEFAULT = "Default";
        public static readonly string ROLE_ADMIN = "Admin";

        //public static readonly int CACHE_DURATION_SECONDS = 600;

    }
}
