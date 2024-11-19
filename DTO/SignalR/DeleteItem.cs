using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO.SignalR
{
	public class DeleteItem
	{
		public Guid itemid {  get; set; }
		public Guid teamid { get; set; }
	}
}
