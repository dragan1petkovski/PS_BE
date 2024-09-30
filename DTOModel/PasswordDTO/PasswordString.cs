using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.PasswordDTO
{
	public class PasswordString
	{
		public Guid? parentid {  get; set; }

		[Required]
		public Guid id { get; set; }
	}
}
