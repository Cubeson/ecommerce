namespace Server.Utility
{
    public static class Constants
    {
        public static readonly double TokenExpirationTimeMinutes = 15d;
        //public static readonly double RefreshTokenExpirationTimeDays = 3d;
        public static readonly double RefreshTokenExpirationTimeHours = 6d;

        public static readonly double PasswordResetLifetimeMinutes = 15d;

        public static readonly string RoleDefault = "Default";
        public static readonly string RoleAdmin = "Admin";

    }
}
