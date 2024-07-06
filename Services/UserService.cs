using DataAccessLayerDB;
using DomainModel.DB;
using DTOModel;
using DTOModel.UserDTO;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace Services
{
    public class UserService
    {
        private readonly PSDBContext _dbContext;

        public UserService(PSDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<UserDBDM> GetAllUsers()
        {

            return _dbContext.Users.ToList();
        }

        public SetStatus AddUser(UserDTO newUser)
        {

            return new SetStatus() { status = "OK" };
        }

        public List<UserDTO> ConvertUserDBDMListToUserFullDTOList(List<UserDBDM> userList)
        {
            List<UserDTO> result = new List<UserDTO>();

            foreach(UserDBDM userDBDM in userList)
            {
                result.Add(new UserDTO()
                {
                    id = userDBDM.id,
                    firstname = userDBDM.firstname,
                    lastname = userDBDM.lastname,
                    username = userDBDM.username,
                    email = userDBDM.email,
                });
            }
            return result;
        }

        public List<UserPartDTO> ConvertUserDBDMListToUserPartDTOList(List<UserDBDM> userList)
        {
            List<UserPartDTO> result = new List<UserPartDTO>();

            foreach (UserDBDM userDBDM in userList)
            {
                Console.WriteLine(userDBDM.Fullname);
                result.Add(new UserPartDTO()
                {
                    id = userDBDM.id,
                    fullname= userDBDM.Fullname,
                    username = userDBDM.username,
                });
            }
            return result;
        }


        private UserDBDM ConvertDTOtoDBDM(UserDTO newUser)
        {
            UserDBDM _newUser = new UserDBDM();
            _newUser.id = Guid.NewGuid();
            _newUser.username = newUser.username;
            _newUser.firstname = newUser.firstname;
            _newUser.lastname = newUser.lastname;
            _newUser.email = newUser.email;

            return _newUser;
        }
    }
}
