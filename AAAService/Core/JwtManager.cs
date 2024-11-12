using DomainModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AAAService.Core
{
	public class JwtManager
	{

		public async Task<string> CreateToken(UserManager<User> _userManager, User _user, IConfiguration _configuration, RoleManager<IdentityRole> _roleManager, ILogger<object> _logger)
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

		private async Task<List<Claim>> GetClaims(User _user, UserManager<User> _userManager, RoleManager<IdentityRole> _roleManager, ILogger<object> _logger)
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

		public JwtPayload GetJWTPayload(string jwt)
		{

			string jwtdata = jwt.Split(" ")[1];
			try
			{
				JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
				JwtSecurityToken t = handler.ReadJwtToken(jwtdata);
				return t.Payload;
			}
			catch
			{
				//In decision making for writing logs here - Malformed jwt token
				return null;
			}


		}

		public bool GetUserID(string jwt, out Guid userid)
		{
			if (Guid.TryParse(GetJWTPayload(jwt)[ClaimTypes.NameIdentifier].ToString(), out Guid _userid))
			{
				userid = _userid;
				return true;
			}
			else
			{
				userid = Guid.Empty;
				return false;
			}

		}

		public (bool userIdSuccess, Guid _userid) GetUserID(string jwt)
		{
			try
			{
				if (Guid.TryParse(GetJWTPayload(jwt)[ClaimTypes.NameIdentifier].ToString(), out Guid _userid))
				{
					return (true, _userid);
				}
				else
				{
					return (false, Guid.Empty);
				}
			}
			catch
			{
				//In decision making for writing logs here
				return (false, Guid.Empty);
			}

		}

		public (bool success, string roleName) GetRoleName(string jwt)
		{
			try
			{
				string roleName = GetJWTPayload(jwt)[ClaimTypes.Role].ToString();
				if (roleName != null)
				{
					return (true, roleName);
				}
				return (false, null);
			}
			catch
			{
				//In decision making for writing logs here
				return (false, null);
			}

		}

	}
}
