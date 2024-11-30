using DTO.User;
using DomainModel;
using DataAccessLayerDB;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace DataMapper
{
	public class UserDataMapper
	{

		public async Task<List<DTO.User.User>> ConvertUserListToUserFullDTOList(List<DomainModel.User> userList, UserManager<DomainModel.User> _userManager)
		{
			return (List<DTO.User.User>)userList.Select(async (u) => new DTO.User.User()
			{
				id = Guid.Parse(u.Id),
				rolename = (await _userManager.GetRolesAsync(u)).FirstOrDefault(),
				firstname = u.firstname,
				lastname = u.lastname,
				username = u.UserName,
				createdate = u.createdate,
				updatedate = u.updatedate,
				email = u.Email
			});
		}

		public List<UserPart> ConvertUserListToUserPartDTOList(List<DomainModel.User> userList)
		{
			return userList.Select(u => new UserPart()
			{
				id = Guid.Parse(u.Id),
				fullname = u.Fullname,
				username = u.UserName,
			}).ToList();
		}

		private DomainModel.User ConvertDTOto(DTO.User.User newUser)
		{
			DomainModel.User _newUser = new DomainModel.User();
			_newUser.Id = Guid.NewGuid().ToString();
			_newUser.UserName = newUser.username;
			_newUser.firstname = newUser.firstname;
			_newUser.lastname = newUser.lastname;
			_newUser.Email = newUser.email;

			return _newUser;
		}

		public async Task<List<DomainModel.User>> GetUsersWithRoleAsync(UserManager<DomainModel.User> _userManager, string rolename, List<DomainModel.User> _users)
		{
			List<DomainModel.User> output = new List<DomainModel.User>();
			foreach(DomainModel.User user in _users)
			{
				if(await _userManager.IsInRoleAsync(user, rolename))
				{
					output.Add(user);
				}
			}
			return output;
		}
	}
}
