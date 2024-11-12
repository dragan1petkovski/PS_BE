using DTO.User;
using DomainModel;
using DataAccessLayerDB;
using Microsoft.AspNetCore.Identity;

namespace DataMapper
{
	public class UserDataMapper
	{

		public List<DTO.User.User> ConvertUserListToUserFullDTOList(List<DomainModel.User> userList)
		{
			List<DTO.User.User> result = new List<DTO.User.User>();

			foreach (DomainModel.User user in userList)
			{
				result.Add(new DTO.User.User()
				{
					id = Guid.Parse(user.Id),
					firstname = user.firstname,
					lastname = user.lastname,
					username = user.UserName,
					createdate = user.createdate,
					updatedate = user.updatedate,
					email = user.Email,
				});
			}
			return result;
		}

		public List<UserPart> ConvertUserListToUserPartDTOList(List<DomainModel.User> userList)
		{
			List<UserPart> result = new List<UserPart>();

			foreach (DomainModel.User user in userList)
			{
				result.Add(new UserPart()
				{
					id = Guid.Parse(user.Id),
					fullname = user.Fullname,
					username = user.UserName,
				});
			}
			return result;
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

		public async Task<List<DomainModel.User>> GetUsersWithRoleAsync(PSDBContext _dbContext, UserManager<DomainModel.User> _userManager, string rolename, List<DomainModel.User> _users)
		{
			List<DomainModel.User> output = new List<DomainModel.User>();
			foreach(DomainModel.User user in _users)
			{
				if(await _userManager.IsInRoleAsync(user,rolename))
				{
					output.Add(user);
				}
			}
			return output;
		}
	}
}
