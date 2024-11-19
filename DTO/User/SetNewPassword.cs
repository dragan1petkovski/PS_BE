using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DTO.Validators;
namespace DTO.User
{
	public class SetNewPassword
	{
		[Required]
		public Guid? requestid {  get; set; }

		[Required]
		[PasswordComplexity]
		public string password {  get; set; }

		[Required]
		[PasswordComplexity]
		public string confirmpassword { get; set; }
	}
}
