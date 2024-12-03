using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Mailjet;
using Mailjet.Client.Resources;


namespace EmailService
{
	public class SMTPClient: iEmailService
	{
		private string _fromEmail;
		private string _serverName;
		private string _username;
		private string _password;
		private int _port;
		private bool _smtpssl;
		public void EmailConfigurationFromAppSettings(IConfiguration configuration, string configSelection)
		{
			try
			{
				_fromEmail = configuration.GetSection($"{configSelection}:email").Value;
				_serverName = configuration.GetSection($"{configSelection}:SMTPserver").Value;
				_username = configuration.GetSection($"{configSelection}:username").Value;
				_password = configuration.GetSection($"{configSelection}:password").Value;
				_port = Int32.Parse(configuration.GetSection($"{configSelection}:SMTPPort").Value);
				_smtpssl = false;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Email Configuration failed with following error:\n {ex.Message}");
			}
		}
		public async Task<bool> SendEmail(string toMailAddress, string emailMessage, string emailSubject)
		{
			using (SmtpClient smtpClient = new SmtpClient())
			{
				MimeMessage message = new MimeMessage();
				message.From.Add(new MailboxAddress("PasswordSense", _fromEmail));
				message.To.Add(new MailboxAddress(toMailAddress, toMailAddress));

				message.Body = new TextPart()
				{
					Text = emailMessage
				};

				message.Subject = emailSubject;

				try
				{

					smtpClient.Connect(_serverName, _port, _smtpssl);
					smtpClient.Authenticate(_username, _password);
					smtpClient.Send(message);
					smtpClient.Disconnect(true);
					return true;
				}
				catch (Exception ex)
				{

					Console.WriteLine("SendMail Error: {0}", ex.Message);
					return false;
				}

			}
		}

		public string GetFromMailAddress()
		{
			return _fromEmail;
		}
	}
}
