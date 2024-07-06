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

        public SetStatus AddUser(PostUserDTO _newUser)
        {
            try
            {
                UserDBDM newUser = new UserDBDM();
                newUser.createdate = DateTime.Now;
                newUser.id = Guid.NewGuid();
                newUser.firstname = _newUser.firstname;
                newUser.lastname = _newUser.lastname;
                newUser.email = _newUser.email;
                newUser.username = _newUser.username;
                newUser.clients = _dbContext.Clients.Where(c => _newUser.clientTeamPairs.Select(ct => ct.clientid).ToList().Any(ct => ct == c.id)).Distinct().ToList();
                newUser.teams = _dbContext.Teams.Where(t => _newUser.clientTeamPairs.Select(ct => ct.teamid).ToList().Any(ct => ct == t.id)).Distinct().ToList();
                newUser.password = "Ovoj text ke se setira poinaku ne preku UI na Administratorot";
                newUser.updatedate = DateTime.Now;
                _dbContext.Users.Add(newUser);
                _dbContext.SaveChanges();
                return new SetStatus() { status = "OK" };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString(),Console.ForegroundColor = ConsoleColor.Red);
                Console.ForegroundColor = ConsoleColor.Black;
                return new SetStatus() { status = "KO" };
                
            }
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
