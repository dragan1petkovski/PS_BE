using AAAService.Interface;
using DataAccessLayerDB;
using DomainModel;
using Microsoft.AspNetCore.Identity;

namespace AAAService.Validators
{
	public class PasswordValidator: iValidator
	{
		private readonly string _userName;
		private readonly string _password;
		private readonly UserManager<User> _userManager;
		private readonly PSDBContext _context;
		public PasswordValidator(string userName, string password, UserManager<User> userManager, PSDBContext context) 
		{
			_userName = userName;
			_password = password;
			_userManager = userManager;
			_context = context;
		}
		public async Task<bool> ProcessAsync()
		{
			User login;
			try
			{
				login = _context.Users.FirstOrDefault(u => u.UserName.ToLower() == _userName.ToLower());
				if (login == null)
				{
					return false;
				}
				return (await _userManager.CheckPasswordAsync(login, _password));
			}
			catch
			(Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}


		}
	}
}
