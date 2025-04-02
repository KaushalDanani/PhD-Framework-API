using Backend.Interfaces;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Net;

namespace Backend.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ISendGridClient _sendGridClient;

        public EmailService(IConfiguration configuration, ISendGridClient sendGridClient)
        {
            _configuration = configuration;
            _sendGridClient = sendGridClient;
        }

        public async Task SendEmailAsync(string emailTo, string confirmLink, string username)
        {
            var from = new EmailAddress("kaushal00095@gmail.com", "no-reply");
            var to = new EmailAddress(emailTo, username);
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "ConfirmEmailTemplate.html");
            var htmlContent = File.ReadAllText(templatePath).Replace("{{confirmationLink}}", confirmLink);
            var subject = "PhD Framework - Confirm Your Email";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = await _sendGridClient.SendEmailAsync(msg);

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                throw new Exception($"Failed to send email. Status Code: {response.StatusCode}");
            }
        }
    }
}
