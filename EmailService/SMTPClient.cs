using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using Mailjet;


namespace EmailService
{
	public class SMTPClient
	{

		public MimeMessage SetNewPasswordMessage(IConfiguration configuration, string sendToMail, string username, string userId)
		{
			MimeMessage message = new MimeMessage();
			message.From.Add(new MailboxAddress("PasswordSense", configuration.GetSection("SmtpSettings:email").Value));
			message.To.Add(new MailboxAddress(sendToMail, sendToMail));

			message.Body = new TextPart()
			{
				Text = "Hello "+username+",\nTo set your password visit the link below.\nhttps://localhost/emailnotification/setnewpassword/" + userId + "\n\n\nKind Regards,\nPasswordSense"
			};

			return message;
		}

		public MimeMessage GetVerificationCode(IConfiguration configuration, string sendToMail, int verificationcode, string username) 
		{
			MimeMessage message = new MimeMessage();
			message.From.Add(new MailboxAddress("PasswordSense", configuration.GetSection("SmtpSettings:email").Value));
			message.To.Add(new MailboxAddress(sendToMail, sendToMail));

			message.Body = new TextPart()
			{
				Text = "Hello " + username + ",\nYour verification code is valid for 1 minute.\n\nCode:  "+verificationcode+"\n\n\nKind Regards,\nPasswordSense"
			};

			return message;
		}

		public bool SendEmailMessage(IConfiguration configuration, MimeMessage message)
		{
			using (SmtpClient smtpClient = new SmtpClient())
			{
				try
				{
					string smtpServer = configuration.GetSection("SmtpSettings:server").Value;
					int smtpPort = int.Parse(configuration.GetSection("SmtpSettings:port").Value);
					bool smtpssl = bool.Parse(configuration.GetSection("SmtpSettings:ssl").Value);
					string email = configuration.GetSection("SmtpSettings:email").Value;
					string passwod = configuration.GetSection("SmtpSettings:emailpassword").Value;
					smtpClient.Connect(smtpServer, smtpPort, smtpssl);
					smtpClient.Authenticate(email, passwod);
					smtpClient.Send(message);
					smtpClient.Disconnect(true);
					return true;
				}
				catch(Exception ex)
				{

					Console.WriteLine("SendMail Error: {0}", ex.Message);
					return false;
				}

			}
		}
	}
}
