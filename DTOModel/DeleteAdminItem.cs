using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel
{
	public class DeleteAdminItem
	{
		[Required]
		public Guid id {  get; set; }

		[Required]
		[RegularExpression(@"[0-9]{8}$")]
		public int verificationCode { get; set; }
	}
}
