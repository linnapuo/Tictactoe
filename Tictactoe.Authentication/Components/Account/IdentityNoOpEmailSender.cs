using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Tictactoe.Authentication.Data;

namespace Tictactoe.Authentication.Components.Account;

// Remove the "else if (EmailSender is IdentityNoOpEmailSender)" block from RegisterConfirmation.razor after updating with a real implementation.
internal sealed class IdentityNoOpEmailSender(ILogger<IdentityNoOpEmailSender> logger) : IEmailSender<ApplicationUser>
{
    private readonly IEmailSender _emailSender = new NoOpEmailSender();

    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        // TODO: https://learn.microsoft.com/en-us/dotnet/core/extensions/logger-message-generator
        logger.LogInformation("{emailConfirmationLink} sent to {email}", confirmationLink, email);
        return _emailSender.SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink) =>
        _emailSender.SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode) =>
        _emailSender.SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
}
