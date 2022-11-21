using Server.Models;
using Shared.DTO;
using System.Net.Mail;

namespace Server.Services.SmtpService;

public interface ISmtpService
{
    public Task UserCreated(User user);
    public Task PasswordResetRequested(RequestResetPassword requestReset, PasswordReset passRst);
}
