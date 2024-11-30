using Mailjet;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace EmailService
{
	public class MailJetMailer: IDisposable, iEmailService
	{
		MailjetClient _client;
		private string _fromMailAddress;

		public string GetFromMailAddress() => _fromMailAddress;
		public void EmailConfigurationFromAppSettings(IConfiguration configuration, string configSelection)
		{
			try
			{
				_client = new MailjetClient(configuration.GetSection($"{configSelection}:apikey").Value, configuration.GetSection($"{configSelection}:secretkey").Value);
				_fromMailAddress = configuration.GetSection($"{configSelection}:email").Value;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Email Configuration failed with following error:\n {ex.Message}");
			}
		}
		public async Task<bool> SendEmail(string toMailAddress, string EmailMessage, string EmailSubject)
		{
			MailjetRequest request = new MailjetRequest
			{
				Resource = Send.Resource,
			}
				.Property(Send.FromEmail, _fromMailAddress)
				.Property(Send.Subject, EmailSubject)
				.Property(Send.TextPart, EmailMessage)
				.Property(Send.To, string.Format("{0} {1}", toMailAddress, toMailAddress));
			MailjetResponse response = null;
			try
			{
				response = await _client.PostAsync(request);
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
