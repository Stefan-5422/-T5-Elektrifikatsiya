using Elektrifikatsiya.Utilities;
using HandlebarsDotNet;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Elektrifikatsiya.Services.Implementations
{
	public class EmailService : IEmailService
	{
		private readonly EmailSettings emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
				this.emailSettings = emailSettings.Value;
        }

        public async Task SendAsync(string to, string subject, string html, string? from = null)
		{
			from ??= emailSettings.DefaultEmail;

            MimeMessage message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(from));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart(TextFormat.Html) { Text = html };

            using SmtpClient smtp = new SmtpClient();
            await smtp.ConnectAsync(emailSettings.SmtpServer, emailSettings.SmtpPort, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(emailSettings.User, emailSettings.Key);
            await smtp.SendAsync(message);
            await smtp.DisconnectAsync(true);
        }

		public Task SendWithTemeplateAsync(string to, string subject, string templateKey, string? from = null, Dictionary<string, string>? templateParameters = null)
		{
			string content = emailSettings.CompiledTemplates[templateKey](templateParameters ?? new());
			return SendAsync(to, subject, content, from);

        }
	}
}
