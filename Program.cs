
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AuthenticationLayer;
using DataAccessLayerDB;
namespace be
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            var connectionstring = builder.Configuration.GetConnectionString("sqlConnection");
            
            builder.Services.AddDbContext<PSDBContext>(option => option.UseSqlServer(connectionstring));

            //builder.Services.AddTransient<PSDBInitializer>();

            //builder.Services.AddIdentityApiEndpoints<IdentityUser>(option =>
            //{
            //    option.User.RequireUniqueEmail = true;
            //    option.Password.RequireDigit = true;
            //    option.Password.RequireLowercase = true;
            //    option.Password.RequireUppercase = true;
            //    option.Password.RequireNonAlphanumeric = true;
            //    option.Password.RequiredLength = 8;
            //    option.Password.RequiredUniqueChars = 8;
            //    option.User.AllowedUserNameCharacters = "zaqxswcdevfrbgtnhymjukilopZAQXSWCDEVFRBGTNHYMJUKILOP1234567890";
            //
            //}).AddRoles<IdentityRole>()
              

            builder.Services.AddAuthentication().AddJwtBearer("jwt");
            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseAuthorization();


            app.MapControllers();

            //Auto Database Initialization
            //using (var scope = app.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //
            //    var initializer = services.GetRequiredService<PSDBInitializer>();
            //
            //    initializer.Run();
            //}

            app.Run();
        }
    }
}
