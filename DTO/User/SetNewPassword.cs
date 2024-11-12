using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.User
{
	public class SetNewPassword
	{
		[Required]
		public Guid? requestid {  get; set; }

		[Required]
		[StringLength(int.MaxValue, MinimumLength = 8)]
		public string password {  get; set; }

		[Required]
		[StringLength(int.MaxValue, MinimumLength = 8)]
		public string confirmpassword { get; set; }
	}
}
