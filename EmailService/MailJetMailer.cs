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

		public Func<string, string, string> SetNewPasswordBody = (string username, string userId) => "Hello " + username + ",\nTo set your password visit the link below.\nhttps://localhost/emailnotification/setnewpassword/" + userId + "\n\n\nKind Regards,\nPasswordSense";

		public Func<string, int, string> GetVerificationCode = (string username, int verificationcode) => "Hello " + username + ",\nYour verification code is valid for 1 minute.\n\nCode:  " + verificationcode + "\n\n\nKind Regards,\nPasswordSense";

		public async Task<bool> SendMailMessage(IConfiguration _configuration, string toMail, string message, string subject)
		{
			try
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
				MailjetResponse response = await client.PostAsync(request);
				if (response.IsSuccessStatusCode)
				{
					Console.WriteLine("Mail Status: {0}",response.StatusCode.ToString());
					return true;
				}
				else
				{
					Console.WriteLine(string.Format("StatusCode: {0}\n", response.StatusCode));
					Console.WriteLine(string.Format("ErrorInfo: {0}\n", response.GetErrorInfo()));
					Console.WriteLine(string.Format("ErrorMessage: {0}\n", response.GetErrorMessage()));
					return false;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("**********Send Mail failed at: {0} ****************\n{1}",DateTime.Now, ex.ToString());
				return false;
			}

		}

		public void Dispose()
		{

		}
	}
}
