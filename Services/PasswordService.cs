using DataAccessLayerDB;
using DTOModel.PasswordDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Services
{
    public class PasswordService
    {
        private readonly PSDBContext _dbContext;

        public PasswordService(PSDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public PasswordDTO GetPasswordById(Guid id)
        {
            return new PasswordDTO() { password = _dbContext.Credentials.Include(c => c.password).Where(c => c.id == id).First().password.password };
        }
    }
}
