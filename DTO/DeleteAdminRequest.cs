using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
	public class DeleteAdminRequest
	{
		[Required]
		public Guid id { get; set; }

		[Required]
		[RegularExpression(@"team$|user$|client$")]
		public string type { get; set; }
	}

}
