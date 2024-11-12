
using System.ComponentModel.DataAnnotations;

namespace DTO.User
{
	public class ChangePassword
	{
		[Required]
		[StringLength(int.MaxValue, MinimumLength = 8)]
		public string newPassword { get; set; }

		[Required]
		[StringLength(int.MaxValue, MinimumLength = 8)]
		public string oldPassword { get; set; }

		[Required]
		[StringLength(int.MaxValue, MinimumLength = 8)]
		public string confirmPassword { get; set; }

		[Required]
		[RegularExpression(@"^[0-9]{8}$")]
		public long verificationcode { get; set; }
	}
}
