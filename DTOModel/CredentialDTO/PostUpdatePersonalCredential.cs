using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.CredentialDTO
{
	public class PostUpdatePersonalCredential
	{
		public Guid id { get; set; }

		[RegularExpression(@"^[A-Za-z0-9_-]*$")]
		[Required]
		public string domain { get; set; }

		[Required]
		[RegularExpression(@"^[A-Za-z0-9_-]*$")]
		public string username { get; set; }

		public string? email { get; set; }

		public string remote {  get; set; }

		public string password { get; set; }

		public string note { get; set; }

		public string? personalfolderid { get; set; }
		public string? originalpersonalfolderid { get; set; }
	}
}
