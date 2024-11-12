using AAAService.Interface;
using AAAService.Core;
using DataAccessLayerDB;
using DomainModel;
using Microsoft.AspNetCore.Identity;

namespace AAAService.Validators
{
	public class TokenValidator: iValidator, IDisposable
	{
		private readonly string _jwtToken;
		private readonly UserManager<User> _context;
		public TokenValidator(string jwtToken, UserManager<User> dbContext) 
		{
			_jwtToken = jwtToken;
			_context = dbContext;
		}

		public async Task<bool> ProcessAsync()
		{
			JwtManager jwtManager = new JwtManager();
			(bool userstatus, Guid _userid) = jwtManager.GetUserID(_jwtToken);
			(bool rolestatus, string roleName) = jwtManager.GetRoleName(_jwtToken);

			if(!userstatus)
			{
				return false;
			}
            if (!rolestatus)
            {
				return false;   
            }

			User user = await _context.FindByIdAsync(_userid.ToString());
			if (user ==null)
			{
				return false;
			}
			if(!(await _context.GetRolesAsync(user)).Any(role => role == roleName))
			{
				return false;
			}

			return true;
		}

		public void Dispose() { }
	}
}
