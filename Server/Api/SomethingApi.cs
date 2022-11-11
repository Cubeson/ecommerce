using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace Server.Api
{
    //public class SomethingApi : IApi
    //{
    //    public void Register(WebApplication app)
    //    {
    //        app.MapPost("/api/Email/test", EmailTest);
    //        app.MapGet("/Hello", HelloTest);
    //    }
    //    public IResult HelloTest([FromServices] ILoggerFactory loggerFactory)
    //    {
    //        var logger = loggerFactory.CreateLogger("Hello");
    //        logger.LogInformation("lol");
    //        return Results.Ok("Hello World!");
    //    }
    //    public IResult EmailTest(string email)
    //    {
    //        using (MailMessage mail = new MailMessage())
    //        {
    //            var credentials = SmtpCredentials.Get();
    //            mail.From = new MailAddress(credentials.Email);
    //            mail.To.Add(email);
    //            mail.Subject = "Test Subject";
    //            mail.Body = "Test Body";
    //            mail.IsBodyHtml = false;
    //            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
    //            {
    //                smtp.Credentials = new System.Net.NetworkCredential(credentials.Email, credentials.Password);
    //                smtp.EnableSsl = true;
    //                smtp.Send(mail);
    //                return Results.Ok("Email sent");
    //            }
    //        }
    //
    //    }
    //}
}
