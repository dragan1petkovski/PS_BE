namespace DomainModel
{

	public enum ActionEnum
	{
		Pending,
		Done,
		Expired
	}

	public enum TypeEnum
	{
		ChangePassword,
		SetNewPassword
	}
	public class EmailNotification
	{
		public Guid Id { get; set; }
		public User user { get; set; }
		public TypeEnum type { get; set; }
		public ActionEnum action { get; set; }
		public bool isClicked { get; set; }
		public long validationCode { get; set; }
		public DateTime createdon { get; set; }
	}
}
