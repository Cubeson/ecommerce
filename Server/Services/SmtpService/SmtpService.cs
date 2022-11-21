using Server.Models;
using Shared.DTO;
using System.Net.Mail;

namespace Server.Services.SmtpService
{
    public class SmtpService : ISmtpService
    {
        private readonly string host = "smtp.gmail.com";
        private readonly int port = 587;
        public Task UserCreated(User user)
        {
            return Task.Run(() =>
            {
                using var mail = new MailMessage();
                var credentials = SmtpSingleton.Get();
                mail.From = new MailAddress(credentials.Email);
                mail.To.Add(user.Email);
                mail.Subject = "Created new account";
                mail.Body = "A new account has been created with this account";
                mail.IsBodyHtml = false;
                using SmtpClient smtp = new SmtpClient(host, port);
                smtp.Credentials = new System.Net.NetworkCredential(credentials.Email, credentials.Password);
                smtp.EnableSsl = true;
                smtp.Send(mail);
            });
        }
        public Task PasswordResetRequested(RequestResetPassword requestReset, PasswordReset passRst)
        {
            return Task.Run(() =>
            {
                using MailMessage mail = new MailMessage();
                var credentials = SmtpSingleton.Get();
                mail.From = new MailAddress(credentials.Email);
                mail.To.Add(requestReset.Email);
                mail.Subject = "Password reset requested";
                mail.Body =
                    "<p>A password reset was requested for an account with your email</p>" +
                    "<p>If you haven't requested a reset, ignore this message</p>" +
                    "<p>If you wish to reset your email, use the code below</p>" +
                    "<p>" + passRst.ResetID + "</p>";
                mail.IsBodyHtml = true;
                using SmtpClient smtp = new SmtpClient(host, port);
                smtp.Credentials = new System.Net.NetworkCredential(credentials.Email, credentials.Password);
                smtp.EnableSsl = true;
                smtp.Send(mail);
            });

        }
    }
}
