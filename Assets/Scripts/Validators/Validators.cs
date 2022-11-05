using System.Net.Mail;
using System;

public static class Validators
{
    public static bool ValidateEmail(string email)
    {
        if (email == null || email.Equals("")) return false;
        try
        {
            MailAddress m = new MailAddress(email);
            // MailAddress jest prosty w obs³udze, ale konstruktor nie jest wystarczaj¹co restrykcyjny
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

