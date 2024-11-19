using AAAService.Interface;
using DataAccessLayerDB;
using DomainModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AAAService.Validators
{
	public class WSGroupValidator: iValidator, IDisposable
	{
		private List<Guid> _teams;
		private Guid _userid;
		private string _rolename;
		private readonly PSDBContext _dbContext;
		private readonly UserManager<User> _userManager;

		public WSGroupValidator(List<Guid> teams, Guid userid, string rolename, PSDBContext dbContext, UserManager<User> userManager)
		{
			_teams = teams;
			_userid = userid;
			_rolename = rolename;
			_dbContext = dbContext;
			_userManager = userManager;
		}

		public async Task<bool> ProcessAsync()
		{
			User user = null;
			try
			{
				user =_dbContext.Users
					.Include(u => u.teams)
					.FirstOrDefault(u => u.Id == _userid.ToString());
			}
			catch
			{
				return false;
			}
			if( user == null)
			{
				return false;
			}
			if(!(await _userManager.GetRolesAsync(user)).Any(r => r == _rolename))
			{
				return false;
			}

			if (user.teams.Where(t => _teams.Any(tid => tid == t.id)).ToList().Count() != _teams.Count())
            {
				return false;
            }

            return true;
		}

		public void Dispose() { }
	}
}
