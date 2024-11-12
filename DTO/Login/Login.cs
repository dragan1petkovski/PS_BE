using System.ComponentModel.DataAnnotations;

namespace DTO.Login
{
	public class Login
	{
		[Required]
		[RegularExpression("^[a-zA-Z0-9]*$")]
		public string username { get; set; }

		[Required]
		public string password { get; set; }
	}
}
