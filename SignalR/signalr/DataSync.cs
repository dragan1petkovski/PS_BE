using AAAService.Core;
using AAAService.Validators;
using DataAccessLayerDB;
using DTO.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Text.Json;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using SignalR.signalr;


namespace SignalR
{
	[Authorize(Roles = "User")]
	public class DataSync: Hub
	{
		
		private readonly UserManager<DomainModel.User> _userManager;
		private readonly PSDBContext _dbContext;

		public DataSync(UserManager<DomainModel.User> userManager, PSDBContext dbContext)
		{
			_userManager = userManager;
			_dbContext = dbContext;
		}

		public async Task RegisterOnClient(string registration, [FromServices] Validation validation)
		{
			Console.WriteLine(registration);
			UserRegister _newRegistration = JsonConvert.DeserializeObject<UserRegister>(registration);

			bool userIdStatus = Guid.TryParse(Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value, out Guid userid);
			string rolename = Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;

			if (!userIdStatus)
			{
				return;
			}
			validation.AddValidator(new WSTeamValidator(_newRegistration.teams, userid, rolename, _dbContext, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return;
			}

			(string connectionid, string type) t = (Context.ConnectionId, _newRegistration.type);
			UserRegistrationStore.AddNewTeam(_newRegistration.teams,t);
		}

	}
}
