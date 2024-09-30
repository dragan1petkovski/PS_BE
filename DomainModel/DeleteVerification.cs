using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModel
{
	public enum ItemType
	{
		client,
		team,
		user,
	}
	public class DeleteVerification
	{
		public Guid id {  get; set; }
		public User requestor { get; set; }
		public ItemType type { get; set; }
		public Guid itemId { get; set; }
		public bool isClicked { get; set; }

		public int verificationCode { get; set; }
		public DateTime createdate { get; set; }
	}
}
