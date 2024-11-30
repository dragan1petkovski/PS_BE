using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
	public class EmailMessages
	{
		private string _baseAPIUrl;

		public EmailMessages(IConfiguration configuration)
		{
			_baseAPIUrl = configuration.GetSection("BasicAPIURL").Value;
		}

		public string Subject = "PasswordSense do not reply";

		public string ResetPasswordBody(string username, string userId) => $"Hello {username},\nTo reset your password visit the link below.\n{_baseAPIUrl}/user/SetNewPassword/{userId}\n\n\nKind Regards,\nPasswordSense";

		public string SetNewPasswordBody(string username, string userId) => $"Hello {username},\nTo set your password visit the link below.\n{_baseAPIUrl}/user/SetNewPassword/{userId}\n\n\nKind Regards,\nPasswordSense";

		public string GetVerificationCode(string username, int verificationcode) => $"Hello {username},\nYour verification code is valid for 1 minute.\n\nCode:  {verificationcode} \n\n\nKind Regards,\nPasswordSense";

	}
}
