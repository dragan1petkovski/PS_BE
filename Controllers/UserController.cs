using Microsoft.AspNetCore.Mvc;
using Services;
using DTOModel.UserDTO;
using DataAccessLayerDB;
using System.Text.Json;

namespace be.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class UserController : ControllerBase
    {
        private readonly PSDBContext _dbContext;

        public UserController(PSDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet()]
        public IEnumerable<UserDTO> GetAllFullUsers()
        {
            UserService userService = new UserService(_dbContext);

            return userService.ConvertUserDBDMListToUserFullDTOList(userService.GetAllUsers());
             
        }

        [HttpGet()]
        public IEnumerable<UserPartDTO> GetAllPartUsers()
        {
            UserService userService = new UserService(_dbContext);

            return userService.ConvertUserDBDMListToUserPartDTOList(userService.GetAllUsers());

        }

        [HttpPost]
        public string AddUser(PostUserDTO user)
        {
            UserService userService = new UserService(_dbContext);
            
            return JsonSerializer.Serialize(userService.AddUser(user));
        }
    }
}
