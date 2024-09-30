using DTOModel.UserDTO;
using DomainModel;
using DataAccessLayerDB;
using Microsoft.AspNetCore.Identity;

namespace DataMapper
{
	public class UserDataMapper
	{

		public List<UserDTO> ConvertUserListToUserFullDTOList(List<User> userList)
		{
			List<UserDTO> result = new List<UserDTO>();

			foreach (User user in userList)
			{
				result.Add(new UserDTO()
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

		public List<UserPartDTO> ConvertUserListToUserPartDTOList(List<User> userList)
		{
			List<UserPartDTO> result = new List<UserPartDTO>();

			foreach (User user in userList)
			{
				result.Add(new UserPartDTO()
				{
					id = Guid.Parse(user.Id),
					fullname = user.Fullname,
					username = user.UserName,
				});
			}
			return result;
		}

		private User ConvertDTOto(UserDTO newUser)
		{
			User _newUser = new User();
			_newUser.Id = Guid.NewGuid().ToString();
			_newUser.UserName = newUser.username;
			_newUser.firstname = newUser.firstname;
			_newUser.lastname = newUser.lastname;
			_newUser.Email = newUser.email;

			return _newUser;
		}

		public async Task<List<User>> GetUsersWithRoleAsync(PSDBContext _dbContext, UserManager<User> _userManager, string rolename, List<User> _users)
		{
			List<User> output = new List<User>();
			foreach(User user in _users)
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
