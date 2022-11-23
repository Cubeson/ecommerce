using Server.Models;
using Shared.DTO;

namespace Server.Services.SmtpService;
public class SmtpTestService : ISmtpService
{
    public Task PasswordResetRequested(RequestResetPasswordDTO requestReset, PasswordReset passRst)
    {
        return Task.CompletedTask;
    }

    public Task UserCreated(User user)
    {
        return Task.CompletedTask;
    }
}
