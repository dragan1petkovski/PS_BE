using DataAccessLayerDB;
using DomainModel;
using DTOModel.TeamDTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using LogginMessages;

namespace AuthenticationLayer
{
	public class UserAuthorization
	{
		public bool IsCertificateUploadAuthorization(List<string> clientteampairs, PSDBContext _dbContext, Guid _userid, out List<ClientTeamPair> pairs, ILogger<UserAuthorization> _logger)
		{
			List<ClientTeamPair> output = new List<ClientTeamPair>();

			User loggedinUser = new User();

			try
			{
				loggedinUser = _dbContext.Users.Include(u => u.teams)
									.Include(u => u.teams).ThenInclude(c => c.client)
									.FirstOrDefault(u => u.Id == _userid.ToString());
			}
			catch(Exception ex)
			{
				_logger.LogCritical(DatabaseLog.DBConnectionLog(ex.ToString()));
			}

			if(loggedinUser != null)
			{
				foreach (string pair in clientteampairs)
				{

					ClientTeamPair _pair = new ClientTeamPair();
					try
					{
						_pair = JsonSerializer.Deserialize<ClientTeamPair>(pair);
					}
					catch
					{
						_logger.LogDebug(EntityLog.InvalidJsonString(pair));
						continue;
					}

					try
					{
						Team team = loggedinUser.teams.FirstOrDefault(c => c.id == _pair.teamid);
						if (team != null)
						{
							output.Add(_pair);
						}
						else
						{
							_logger.LogDebug(EntityLog.NotFound("Team",_pair.teamid.ToString()));
						}
					}
					catch (Exception ex)
					{
						_logger.LogError(DatabaseLog.DBConnectionLog(ex.ToString()));
					}

				}
				pairs = output;
				return true;
			}
			else
			{
				_logger.LogDebug(EntityLog.NotFound("User",loggedinUser.Id));
				pairs = new List<ClientTeamPair>();
				return false;
			}


		}
	
		public async Task<bool> IsValidLoggedUser(UserManager<User> _userManager, RoleManager<IdentityRole> _roleManager,  JwtTokenManager _jwtTokenManager, string jwt, ILogger<UserAuthorization> _logger)
		{
			if(_jwtTokenManager.GetUserID(jwt, out Guid _userid))
			{
				User loggedUser = await _userManager.FindByIdAsync(_userid.ToString());
				if (loggedUser != null)
				{
					string rolename = await _jwtTokenManager.GetRoleName(jwt, _roleManager);
					if ((await _userManager.GetRolesAsync(loggedUser)).Any(r => r == rolename))
					{
						return true;
					}
					else
					{
						_logger.LogWarning(EntityLog.SecurityTokenForgered($"Role {rolename} don't exit"));
						_logger.LogDebug(EntityLog.SecurityTokenForgered(jwt));
						return false;
					}
					
				}
				else
				{
					_logger.LogWarning(EntityLog.SecurityTokenForgered(null));
					_logger.LogDebug(EntityLog.SecurityTokenForgered(jwt));
					return false;
				}
				
			}
			else
			{
				_logger.LogDebug(EntityLog.InvalidJWTToken());
				return false;
			}
			
		}

		public async Task<bool> IsLoggedUserAdmin(UserManager<User> _userManager, JwtTokenManager _jwtTokenManager, string jwt, ILogger<UserAuthorization> _logger)
		{
			if (_jwtTokenManager.GetUserID(jwt, out Guid _userid))
			{
				User loggedUser = await _userManager.FindByIdAsync(_userid.ToString());
				if (loggedUser != null)
				{
					if ((await _userManager.GetRolesAsync(loggedUser)).Any(r => r == "Administrator"))
					{
						return true;
					}
					else
					{
						return false;
					}

				}
				else
				{
					_logger.LogWarning(EntityLog.SecurityTokenForgered(null));
					_logger.LogDebug(EntityLog.SecurityTokenForgered(jwt));
					return false;
				}

			}
			else
			{
				_logger.LogDebug(EntityLog.InvalidJWTToken());
				return false;
			}
		}
	}
}
