using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.PersonalDTO
{
	public class PostDeletePersonalCredential
	{
		public Guid id {  get; set; }
		public Guid? userid { get; set; }
		public Guid? personalfolderid { get; set; }
	}
}
