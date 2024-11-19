using AAAService.Interface;
using AAAService.Core;
using DataAccessLayerDB;
using DomainModel;
using Microsoft.AspNetCore.Identity;

namespace AAAService.Validators
{
	public class TokenValidatorWS: iValidator, IDisposable
	{
		private readonly string _rolename;
		private readonly Guid _userid;
		private readonly UserManager<User> _context;
		public TokenValidatorWS(Guid userid, string rolename, UserManager<User> dbContext) 
		{
			_userid = userid;
			_rolename = rolename;
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

			if(!(await _context.GetRolesAsync(user)).Any(role => role == _rolename))
			{
				return false;
			}

			return true;
		}

		public void Dispose() { }
	}
}
