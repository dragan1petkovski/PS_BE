using AAAService.Core;
using AAAService.Interface;
using DomainModel;
using Microsoft.AspNetCore.Identity;

namespace AAAService.Validators
{
	public class IsUserAdmin: iValidator, IDisposable
	{
		private readonly string _jwtToken;
		private readonly UserManager<User> _context;
		public IsUserAdmin(string jwtToken, UserManager<User> dbContext)
		{
			_jwtToken = jwtToken;
			_context = dbContext;
		}

		public async Task<bool> ProcessAsync()
		{
			JwtManager jwtManager = new JwtManager();
			(bool validUserId, Guid _userid) = jwtManager.GetUserID(_jwtToken);
			if (!validUserId)
			{
				return false;
			}

			User loggedUser = await _context.FindByIdAsync(_userid.ToString());
			if (loggedUser == null)
			{
				return false;
			}

			if (!(await _context.GetRolesAsync(loggedUser)).Any(r => r == "Administrator"))
			{
				return false;
			}

			return true;
		}

		public void Dispose() { }
	}
}
