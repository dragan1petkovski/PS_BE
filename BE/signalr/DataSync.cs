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
using SignalR;


namespace SignalR
{
	[Authorize(Roles = "User, Administrator")]
	public class DataSync: Hub
	{
		
		private readonly UserManager<DomainModel.User> _userManager;
		private readonly PSDBContext _dbContext;

		public DataSync(UserManager<DomainModel.User> userManager, PSDBContext dbContext)
		{
			_userManager = userManager;
			_dbContext = dbContext;
		}

		public async Task RegisterUserLocation(string registration, [FromServices] Validation validation)
		{
			UserRegister _newRegistration = JsonConvert.DeserializeObject<UserRegister>(registration);
			bool userIdStatus = Guid.TryParse(Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value, out Guid userid);
			string rolename = Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;

			if (!userIdStatus)
			{
				return;
			}

			if (_newRegistration.type != "personal")
			{
				validation.AddValidator(new WSGroupValidator(_newRegistration.groups, userid, rolename, _dbContext, _userManager));
				if (!(await validation.ProcessAsync()))
				{
					return;
				}

				foreach (Guid group in _newRegistration.groups)
				{
					Groups.AddToGroupAsync(Context.ConnectionId, JsonConvert.SerializeObject(new { id = group, type = _newRegistration.type }));
				}
			}
			else
			{
				validation.AddValidator(new TokenValidatorWS(userid, rolename, _userManager));
				if(!(await validation.ProcessAsync()))
				{
					return;
				}
				Groups.AddToGroupAsync(Context.ConnectionId, JsonConvert.SerializeObject(new {id = _newRegistration.groups, type=_newRegistration.type}));
			}

		}


		public async Task RegisterAdminLocation(string registration, [FromServices] Validation validation)
		{
			AdminRegister _newRegistration = JsonConvert.DeserializeObject<AdminRegister>(registration);
			bool userIdStatus = Guid.TryParse(Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value, out Guid userid);
			string rolename = Context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role).Value;

			if (!userIdStatus)
			{
				return;
			}
			validation.AddValidator(new TokenValidatorWS(userid, rolename, _userManager));
			validation.AddValidator(new IsUserAdminWS(userid, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return;
			}

			Groups.AddToGroupAsync(Context.ConnectionId, _newRegistration.type);
		}
	}
}
