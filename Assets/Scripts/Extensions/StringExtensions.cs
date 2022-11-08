namespace Assets.Scripts.Extensions
{
    internal static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str)
        {
            return str == null || str.Length == 0;
        }
    }
}
