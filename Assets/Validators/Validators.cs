using System;
using System.Net.Mail;

namespace Shared.Validators
{
    public static class Validators
    {
        public static bool isValidEmail(string email)
        {
            if (email == null || email.Equals("")) return false;
            try
            {
                MailAddress m = new MailAddress(email);
                // MailAddress jest prosty w obsłudze, ale konstruktor nie jest wystarczająco restrykcyjny
                return m.Address.Equals(email);
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public static bool IsValidPassword(string password)
        {
            if (password == null) return false;
            if (password.Length < 8) return false;
            return true;
        }
    }
}