using DomainModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransitionObjectMapper;

namespace DataMapper
{
	public class PasswordDataMapper
	{
		public SymmetricKey ConvertPasswordToSymmetricKey(Password password)
		{
			SymmetricKey output = new SymmetricKey();
			output.password = password.password;
			output.aad = password.aad;
			return output;
		}
	}
}
