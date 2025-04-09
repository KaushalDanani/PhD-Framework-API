namespace Backend.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string emailTo, string confirmLink, string username);
        Task SendPasswordResetEmail(string emailTo, string passwordResetLink, string username);
        Task SendTemporaryCredentialEmailAsync(string emailTo, string passwordResetLink, string username, string designation, string password);
    }
}
