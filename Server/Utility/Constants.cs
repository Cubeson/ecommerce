namespace Server.Utility
{
    public static class Constants
    {
        public static readonly double TOKEN_EXPIRATION_TIME_MINUTES = 15d;
        public static readonly double REFRESH_TOKEN_EXPIRATION_TIME_HOURS = 6d;

        public static readonly double PASSWORD_RESET_LIFETIME_MINUTES = 15d;

        public static readonly string ROLE_DEFAULT = "Default";
        public static readonly string ROLE_ADMIN = "Admin";

        public static readonly int CACHE_DURATION_SECONDS = 600;

    }
}
