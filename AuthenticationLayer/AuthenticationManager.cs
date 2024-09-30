using DomainModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

using System.IdentityModel.Tokens.Jwt;
using DTOModel.LoginDTO;
using Microsoft.EntityFrameworkCore;
using DataAccessLayerDB;
using EncryptionLayer;
using System.Net;
using System.Web;
using Microsoft.Extensions.Logging;

namespace AuthenticationService
{
	public class AuthenticationManager
	{
		public async Task<User> ValidateUser(LoginDTO loginUser, UserManager<User> _userManager, ILogger<AuthenticationManager> _logger)
		{
			try
			{
				User _user = await _userManager.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginUser.username.ToLower());
				if (_user != null)
				{
					if(await _userManager.CheckPasswordAsync(_user, loginUser.password))
					{
						return _user;
					}
					else
					{
						_logger.LogDebug("User: {0} - Incorrect password\n", loginUser.username);
						return null;
					}
					
				}
				else
				{
					_logger.LogDebug("User: {0} - Does NOT Exist", loginUser.username);
					return null;
				}
			}
			catch (Exception ex)
			{
				_logger.LogCritical("{0}: Cannot connect to the database\nDetails:\n{1}\n\n", DateTime.Now,ex.ToString());
				return null;
			}
			

			
		}

		public async Task<string> CreateToken(UserManager<User> _userManager, User _user, IConfiguration _configuration, RoleManager<IdentityRole> _roleManager, ILogger<AuthenticationManager> _logger)
		{
			try
			{
				SigningCredentials _signingCredentials = GetSigningCredentials(_configuration);
				List<Claim> _claims = await GetClaims(_user, _userManager, _roleManager,_logger);
				JwtSecurityToken _jwtToken = GenerateTokenOptions(_signingCredentials, _claims, _configuration);
				return new JwtSecurityTokenHandler().WriteToken(_jwtToken);
			}
			catch
			{
				_logger.LogCritical($"{DateTime.Now} - Security Token can not be created");
				return null;
			}
		}
		private SigningCredentials GetSigningCredentials(IConfiguration _configuration)
		{
			SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));
			return new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
		}

		private async Task<List<Claim>> GetClaims(User _user, UserManager<User> _userManager, RoleManager<IdentityRole> _roleManager, ILogger<AuthenticationManager> _logger)
		{
			List<Claim> claims = new List<Claim>();
			List<string> roles = new List<string>();

			try
			{
				roles = (List<string>)await _userManager.GetRolesAsync(_user);
			}
			catch (Exception ex)
			{
				_logger.LogDebug("{0}: Cannot connect to the database\nDetails:\n{1}\n\n", DateTime.Now, ex.ToString());
			}

			foreach(var role in roles)
			{
				IdentityRole _role = new IdentityRole();
				
				try
				{
					_role = await _roleManager.FindByNameAsync(role);
				}
				catch (Exception ex)
				{
					_logger.LogDebug("{0}: Cannot connect to the database\nDetails:\n{1}\n\n", DateTime.Now, ex.ToString());
				}

				claims.Add(new Claim(ClaimTypes.Role, _role.Name ));
			}
			claims.Add(new Claim(ClaimTypes.NameIdentifier, _user.Id));
			
			return claims;
		}

		private JwtSecurityToken GenerateTokenOptions(SigningCredentials _signingCredentials, List<Claim> _claims, IConfiguration _configuration)
		{

			JwtSecurityToken output = new JwtSecurityToken(
				issuer: _configuration.GetSection("Jwt").GetSection("Issuer").Value,
				claims: _claims,
				expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration.GetSection("Jwt").GetSection("Expires").Value)),
				signingCredentials: _signingCredentials,
				audience: _configuration.GetSection("Jwt").GetSection("Audience").Value
	
			) ;
			return output ;
		}
	
		

	}
}
