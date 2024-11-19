using AAAService.Interface;
using AAAService.Core;
using DataAccessLayerDB;
using DomainModel;
using Microsoft.AspNetCore.Identity;

namespace AAAService.Validators
{
	public class IsUserAdminWS : iValidator, IDisposable
	{
		private readonly Guid _userid;
		private readonly UserManager<User> _context;
		public IsUserAdminWS(Guid userid, UserManager<User> dbContext) 
		{
			_userid = userid;
			_context = dbContext;
		}

		public async Task<bool> ProcessAsync()
		{
			JwtManager jwtManager = new JwtManager();
			DomainModel.User user = await _context.FindByIdAsync(_userid.ToString());
			if(user == null)
			{
				return false;
			}

			if(!(await _context.GetRolesAsync(user)).Any(role => role == "Administrator"))
			{
				return false;
			}

			return true;
		}

		public void Dispose() { }
	}
}
