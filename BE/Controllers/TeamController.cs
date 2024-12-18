﻿using DataAccessLayerDB;
using DataMapper;
using DomainModel;
using DTO;
using DTO.Team;
using DTO.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AppServices;
using System.Text.Json;
using AAAService.Core;
using AAAService.Validators;
using LogginMessages;
using Org.BouncyCastle.Ocsp;
using Newtonsoft.Json;
using SignalR;
using Microsoft.AspNetCore.SignalR;

namespace BE.Controllers
{
    [ApiController]
	[ResponseCache(NoStore = true)]
    public class TeamController : ControllerBase
    {
        private readonly PSDBContext _dbContext;
        private readonly TeamService _service;
        private readonly TeamDataMapper _dataMapper;
        private readonly UserManager<DomainModel.User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtManager _jwtManager;
		private readonly IHubContext<DataSync> _hubContext;

        public TeamController(PSDBContext dbContext, TeamService teamService, TeamDataMapper teamDataMapper, JwtManager jwtManager, UserManager<DomainModel.User> userManager, RoleManager<IdentityRole> roleManager, IHubContext<DataSync> hubContext)
        {
            _dbContext = dbContext;
			_service = teamService;
            _dataMapper = teamDataMapper;
            _jwtManager = jwtManager;
            _userManager = userManager;
            _roleManager = roleManager;
			_hubContext = hubContext;

        }

		#region GET Methods
		[HttpGet("[controller]/{id:guid?}")]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> Read(Guid? id, [FromServices] RoleManager<IdentityRole> _roleManager, [FromServices] UserDataMapper _userDataMapper, [FromServices] Validation validation)
		{
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(403, StatusMessages.AccessDenied);
			}
			if (!id.HasValue)
			{
				(StatusMessages statusCode, var output) = _service.GetAllTeams(_dbContext, _dataMapper);
				if(StatusMessages.Ok == statusCode)
				{
					return StatusCode(statusCode, output);
				}
				return StatusCode(statusCode, statusCode);
			}
			else
			{
				(StatusMessages statusCode, GetTeamUpdate output) = await _service.Update(_dbContext, id.HasValue ? id.Value : Guid.Empty, _userDataMapper, _userManager);
				return StatusCode(statusCode, output);
			}
		}

		[HttpGet("[controller]/mapping/{type?}")]
		[Authorize(Roles = "Administrator, User")]
		public async Task<IActionResult> ClientTeamMappings(string? type, [FromServices] RoleManager<IdentityRole> _roleManager, [FromServices] UserDataMapper _userDataMapper, [FromServices] Validation validation)
		{
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(403, StatusMessages.AccessDenied);
			}
			(_, Guid userid) = _jwtManager.GetUserID(Request.Headers.Authorization);
			Console.WriteLine(type);
			if (type == "all")
			{
				(int statusCode, dynamic output) = _service.GetAllClientTeamMappings(_dbContext);
				if (StatusMessages.Ok == statusCode)
				{
					return StatusCode(statusCode, output);
				}
				return StatusCode(statusCode, statusCode);
			}
			else
			{
				(int statusCode, List<ClientTeamMapping> output) = _service.GetAllClientTeamMappingsByUserId(userid, _dbContext, _dataMapper);
				if (StatusMessages.Ok == statusCode)
				{
					return StatusCode(statusCode, output);
				}
				return StatusCode(statusCode, statusCode);
			}
			

		}

		#endregion
		
		[HttpPost("[controller]")]
		[Authorize(Roles = "Administrator")]
		public async Task<IActionResult> Create(PostTeam _newTeam, [FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}
			(StatusMessages status, DTO.Team.Team data) = _service.Create(_newTeam,_dbContext);
			if(StatusMessages.AddNewTeam == status)
			{
				_hubContext.Clients.Group("team").SendAsync("AdminNotification", JsonConvert.SerializeObject(new { status = "new", type = "team", data = data }));
			}
			return StatusCode(status, status);

		}
        

        [HttpPut("[controller]")]
        [Authorize(Roles ="Administrator")]
        public async Task<IActionResult> Update(PostTeamUpdate update, [FromServices] Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization, _userManager));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(StatusMessages.AccessDenied, StatusMessages.AccessDenied);
			}
            (StatusMessages status, DTO.Team.Team data)  = _service.Update(update, _dbContext);
			if(StatusMessages.UpdateTeam == status)
			{
				_hubContext.Clients.Group("team").SendAsync("AdminNotification", JsonConvert.SerializeObject(new { status = "update", type = "team", data = data }));
			}
			return StatusCode(status, status);

		}

		[HttpDelete("[controller]/{itemid:guid}/{verificateionCode:int}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(Guid itemid, int verificateionCode, [FromServices] IConfiguration _configuration, [FromServices] CertificateService _certificateService, [FromServices] EmailNotificationService _emailNotificationService, [FromServices] AAAService.Core.Validation validation)
        {
			validation.AddValidator(new TokenValidator(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new IsUserAdmin(Request.Headers.Authorization, _userManager));
			validation.AddValidator(new DeleteCodeValidator(_dbContext, _configuration, new DeleteAdminItem() { id = itemid, verificationCode = verificateionCode }));
			if (!(await validation.ProcessAsync()))
			{
				return StatusCode(403,StatusMessages.AccessDenied);
			}
			DeleteVerification deleteVerification = _dbContext.deleteVerifications.FirstOrDefault(dv => dv.itemId == itemid && dv.isClicked);
			if (deleteVerification == null)
			{
				return StatusCode(StatusMessages.InvalidVerificationCode, (string)StatusMessages.InvalidVerificationCode);
			}

			_dbContext.deleteVerifications.Remove(deleteVerification);
			_dbContext.SaveChanges();
			StatusMessages status = _service.Delete(itemid, _certificateService, _dbContext, _configuration);
			if(status == StatusMessages.DeleteTeam)
			{
				_hubContext.Clients.Group("team").SendAsync("AdminNotification", JsonConvert.SerializeObject(new { status = "delete", type = "team", data = itemid }));
			}
			return StatusCode(status, status);
        }
    }
}
