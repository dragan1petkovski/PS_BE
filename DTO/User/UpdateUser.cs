using DTO.Team;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.User
{
	public class UpdateUser
	{
		public Guid id {  get; set; }
		public string firstname { get; set; }
		public string lastname { get; set; }
		public string email { get; set; }
		public string username { get; set; }

		public List<ClientTeamMapping> clientTeamMapping{ get; set; }
	}
}
