using Mailjet;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace EmailService
{
	public class MailJetMailer: IDisposable
	{
		public string Subject = "PasswordSense do not reply";

		public Func<string, string, string> ResetPasswordBody = (string username, string userId) => "Hello " + username + ",\nTo reset your password visit the link below.\nhttps://localhost/user/SetNewPassword/" + userId + "\n\n\nKind Regards,\nPasswordSense";

		public Func<string, string, string> SetNewPasswordBody = (string username, string userId) => "Hello " + username + ",\nTo set your password visit the link below.\nhttps://localhost/user/SetNewPassword/" + userId + "\n\n\nKind Regards,\nPasswordSense";

		public Func<string, int, string> GetVerificationCode = (string username, int verificationcode) => "Hello " + username + ",\nYour verification code is valid for 1 minute.\n\nCode:  " + verificationcode + "\n\n\nKind Regards,\nPasswordSense";

		public async Task<bool> SendMailMessage(IConfiguration _configuration, string toMail, string message, string subject)
		{
			MailjetClient client = new MailjetClient(_configuration.GetSection("MailJetSettings:apikey").Value, _configuration.GetSection("MailJetSettings:secretkey").Value);
			MailjetRequest request = new MailjetRequest
			{
				Resource = Send.Resource,
			}
			   .Property(Send.FromEmail, _configuration.GetSection("MailJetSettings:email").Value)
			   .Property(Send.Subject, subject)
			   .Property(Send.TextPart, message)
			   .Property(Send.To, string.Format("{0} {1}", toMail, toMail));
			MailjetResponse response = null;
			try
			{
				response = await client.PostAsync(request);
			}
			catch
			{
				//Log Event
				return false;
			}

			return response.IsSuccessStatusCode;
		}

		public void Dispose()
		{

		}
	}
}
