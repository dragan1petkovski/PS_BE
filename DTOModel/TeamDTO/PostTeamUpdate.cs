using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOModel.TeamDTO
{
	public class PostTeamUpdate
	{
		[Required]
		public Guid Id { get; set; }

		[Required]
		[RegularExpression(@"^[a-zA-Z0-9_-]*$")]
		public string name { get; set; }

		public List<Guid>? userIds { get; set; }
	}
}
