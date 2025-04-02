namespace Backend.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string emailTo, string confirmLink, string username);
    }
}
