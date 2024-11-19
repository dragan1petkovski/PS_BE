using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.SignalR
{
	public class UserRegister
	{
		public List<Guid>? groups {  get; set; }
		public string type { get; set; }

	}
}
