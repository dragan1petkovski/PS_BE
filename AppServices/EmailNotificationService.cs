using DTO;
using DataAccessLayerDB;
using DomainModel;

namespace AppServices
{
	public class EmailNotificationService
	{

		public bool ValidTime(TimeSpan time)
		{
			if (time.Minutes == 0 && time.Hours == 0 && time.Seconds > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool TryValidateDeleteCode(PSDBContext _dbContext , DeleteAdminItem deleteItem, out Guid _deleteVerificationId)
		{
			DeleteVerification deleteVerification = _dbContext.deleteVerifications.FirstOrDefault(dv => dv.verificationCode == deleteItem.verificationCode && dv.itemId == deleteItem.id);
			if (deleteVerification != null && ValidTime(DateTime.Now - deleteVerification.createdate))
			{
				_deleteVerificationId = deleteVerification.id;
				return true;
			}
			else
			{
				_deleteVerificationId = Guid.Empty;
				return false;
			}
			
		}
	}
}
