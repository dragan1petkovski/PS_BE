using System.ComponentModel.DataAnnotations;

namespace DTO.Password
{
	public class PasswordString
	{
		public Guid? parentid {  get; set; }

		[Required]
		public Guid id { get; set; }
	}
}
