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
            var htmlContent = File.ReadAllText(templatePath)
                .Replace("{{confirmationLink}}", confirmLink)
                .Replace("{{userName}}", username);
            var subject = "PhD Framework - Confirm Your Email";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = await _sendGridClient.SendEmailAsync(msg);

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                throw new Exception($"Failed to send email. Status Code: {response.StatusCode}");
            }
        }

        public async Task SendPasswordResetEmail(string emailTo, string passwordResetLink, string username)
        {
            var from = new EmailAddress("kaushal00095@gmail.com", "no-reply");
            var to = new EmailAddress(emailTo);
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "PasswordResetTemplate.html");
            var htmlContent = File.ReadAllText(templatePath)
                .Replace("{{resetPasswordLink}}", passwordResetLink)
                .Replace("{{userName}}", username);
            var subject = "PhD Framework - Reset your password";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = await _sendGridClient.SendEmailAsync(msg);

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                throw new Exception($"Failed to send email. Status Code: {response.StatusCode}");
            }
        }

        public async Task SendTemporaryCredentialEmailAsync(string emailTo, string passwordResetLink, string username, string designation, string password)
        {
            var from = new EmailAddress("kaushal00095@gmail.com", "no-reply");
            var to = new EmailAddress(emailTo, username);
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "TemporaryAccountCredentialTemplate.html");
            var htmlContent = File.ReadAllText(templatePath)
                .Replace("{{resetPasswordLink}}", passwordResetLink)
                .Replace("{{userName}}", username)
                .Replace("{{Designation}}", designation)
                .Replace("{{email}}", emailTo)
                .Replace("{{password}}", password);
            var subject = "PhD Framework - Temporary account credential";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = await _sendGridClient.SendEmailAsync(msg);

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                throw new Exception($"Failed to send email. Status Code: {response.StatusCode}");
            }
        }

        public async Task SendProgressReportUpdatedByStudentEmailToGuideAsync(string emailTo, string guideUserName, string studentName,
            string registrationNo, string studentEmail, string phdTitle, string researchArea, string newUpdatedReportFilePath,
            string reportFileName)
        {
            var from = new EmailAddress("kaushal00095@gmail.com", "no-reply");
            var to = new EmailAddress(emailTo, guideUserName);
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "ProgressReportUpdatedByStudentEmailTemplate.html");
            var htmlContent = File.ReadAllText(templatePath)
                .Replace("{{userName}}", guideUserName)
                .Replace("{{viewNewReport}}", newUpdatedReportFilePath)
                .Replace("{{studentName}}", studentName)
                .Replace("{{registrationNo}}", registrationNo)
                .Replace("{{phdTitle}}", phdTitle)
                .Replace("{{reportFileName}}", reportFileName)
                .Replace("{{researchArea}}", researchArea)
                .Replace("{{studentEmail}}", studentEmail);
            var subject = "PhD Framework Update – Student Altered Progress Report; Please Review";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            var response = await _sendGridClient.SendEmailAsync(msg);

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                throw new Exception($"Failed to send email. Status Code: {response.StatusCode}");
            }
        }
    }
}
