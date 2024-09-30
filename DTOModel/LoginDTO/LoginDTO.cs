using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.LoginDTO
{
	public class LoginDTO
	{
		[Required]
		[RegularExpression("^[a-zA-Z0-9]*$")]
		public string username {  get; set; }

		[Required]
		public string password { get; set; }
	}
}
