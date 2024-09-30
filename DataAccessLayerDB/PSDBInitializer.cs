using DomainModel;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using EncryptionLayer;
using Microsoft.Extensions.Configuration;
using System.Text;
using TransitionObjectMapper;

namespace DataAccessLayerDB
{
    public class PSDBInitializer
    {
        private readonly PSDBContext _context;
        private readonly IConfiguration _configuration;

        public PSDBInitializer(PSDBContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public void Run()
        {

            PasswordHasher<User> passwordHasher = new PasswordHasher<User>();
			SymmetricEncryption se = new SymmetricEncryption();

			IdentityRole adminRole = new IdentityRole();
            adminRole.Name = "Administrator";
            adminRole.NormalizedName = adminRole.Name.Normalize();
            adminRole.Id = Guid.NewGuid().ToString();
            
            IdentityRole userRole = new IdentityRole();
            userRole.Name = "User";
            userRole.NormalizedName = userRole.Name.Normalize();
            userRole.Id = Guid.NewGuid().ToString();
            
            User admin = new User();
            admin.firstname = "admin";
            admin.lastname = "admin";
            admin.UserName = "Administrator";
            admin.NormalizedUserName = admin.UserName.Normalize();
            admin.Id = Guid.NewGuid().ToString();
            admin.Email = "admin@admin.com";
            admin.NormalizedEmail = admin.Email.Normalize();
            admin.PasswordHash = passwordHasher.HashPassword(admin, "ZAQ!xsw2");
            admin.createdate = DateTime.Now;
			admin.updatedate = DateTime.Now;

			IdentityUserRole<string> ar = new IdentityUserRole<string>();
			ar.RoleId = adminRole.Id;
			ar.UserId = admin.Id;



			_context.Users.AddRange(admin);
            _context.Roles.AddRange(userRole, adminRole);
			_context.UserRoles.AddRange(ar);
            _context.SaveChanges();
        }
    }
}
