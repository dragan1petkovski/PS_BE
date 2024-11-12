using AAAService.Interface;
using DataAccessLayerDB;
using DomainModel;
using DTO;
using Microsoft.Extensions.Configuration;


namespace AAAService.Validators
{
	public class DeleteCodeValidator: iValidator
	{
		private readonly PSDBContext _dbContext;
		private readonly DeleteAdminItem _deleteItem;
		private readonly IConfiguration _configuration;

		public DeleteCodeValidator(PSDBContext dbContext, IConfiguration configuration, DeleteAdminItem deleteItem)
		{
			_dbContext = dbContext;
			_deleteItem = deleteItem;
			_configuration = configuration;
		}
		public async Task<bool> ProcessAsync()
		{
			DeleteVerification deleteVerification = null;
			try
			{
				deleteVerification = _dbContext.deleteVerifications.FirstOrDefault(dv => dv.verificationCode == _deleteItem.verificationCode && dv.itemId == _deleteItem.id && !dv.isClicked);
			}
			catch
			{
				return false;
			}
			if (deleteVerification == null)
			{
				return false;
			}
			TimeValidator timeValidator = new TimeValidator(deleteVerification.createdate, deleteVerification.createdate.AddMinutes(int.Parse(_configuration.GetSection("DeleteAdminItems:ExpirationTime").Value)));
			if( await timeValidator.ProcessAsync())
			{
				deleteVerification.isClicked = true;
				try
				{
					_dbContext.deleteVerifications.Update(deleteVerification);
					_dbContext.SaveChanges();
					return true;
				}
				catch
				{
					return false;
				}
			}
			else
			{
				return false;
			}
		}
	}
}
