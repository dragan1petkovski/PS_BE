using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
	public interface iEmailService
	{
		public void EmailConfigurationFromAppSettings(IConfiguration configuration, string configSelection);
		public Task<bool> SendEmail(string toMailAddress, string emailMessage, string emailSubject);

	}
}
