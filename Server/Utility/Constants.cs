namespace Server.Utility
{
    public static class Constants
    {
        public static readonly string FileModelString = "model";
        public static readonly string FilethumbnailString = "thumbnail";
        public static readonly string FileArchiveString = "archive";

        public static readonly double TokenExpirationTimeMinutes = 15d;
        public static readonly double RefreshTokenExpirationTimeDays = 3d;

        public static readonly double PasswordResetLifetimeMinutes = 15d;

    }
}
