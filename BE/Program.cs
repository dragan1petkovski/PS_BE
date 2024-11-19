using DomainModel;
using DataAccessLayerDB;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using AAAService.Core;
using AppServices;
using DataMapper;
using EmailService;
using EncryptionLayer;
using SignalR;

namespace BE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(builder.Configuration)
				.Enrich.FromLogContext()
				.CreateLogger();

			builder.Host.UseSerilog((context, services, configuration) => configuration
										.ReadFrom.Configuration(context.Configuration)
										.ReadFrom.Services(services)
										);

			builder.Services.AddCors();

			builder.Services.AddSignalR(options => {
				options.KeepAliveInterval = TimeSpan.FromSeconds(15);
			});

			// Add services to the container.
			string connectionstring = string.Empty;
			builder.Services.AddControllers();
            
            try
            {
				connectionstring = builder.Configuration.GetConnectionString("sqlConnection");
			}
            catch (Exception ex)
            {
                Log.Fatal("AppSettings.json file is missing OR 'sqlConnection' key is missing from the configuration file - {0}\nDetails:\n{1}\n\n", DateTime.Now,ex.ToString());
			}
            
            try
            {
				builder.Services.AddDbContext<PSDBContext>(option => option.UseSqlServer(connectionstring));
			}
            catch(Exception ex)
            {
				Log.Fatal("Can NOT connect to the database - {0}\nDetails:\n{1}\n\n", DateTime.Now, ex.ToString());
			}

            builder.Services.AddScoped<Validation>();
            builder.Services.AddScoped<JwtManager>();
			builder.Services.AddScoped<UserService>();
			builder.Services.AddScoped<UserDataMapper>();

			builder.Services.AddScoped<CredentialService>();
			builder.Services.AddScoped<CredentialDataMapper>();

			builder.Services.AddScoped<ClientService>();
			builder.Services.AddScoped<ClientDataMapper>();

			builder.Services.AddScoped<TeamService>();
			builder.Services.AddScoped<TeamDataMapper>();


			builder.Services.AddScoped<CertificateService>();
			builder.Services.AddScoped<CertificateDataMapper>();

			builder.Services.AddScoped<PersonalService>();

			builder.Services.AddScoped<PasswordService>();
			builder.Services.AddScoped<PasswordDataMapper>();
			builder.Services.AddScoped<SymmetricEncryption>();

			builder.Services.AddScoped<EmailNotificationService>();
			builder.Services.AddScoped<MailJetMailer>();
            builder.Services.AddScoped<ClientNotification>();
			//builder.Services.AddTransient<PSDBInitializer>();

			

			builder.Services.AddIdentityApiEndpoints<User>(option =>
            {
                option.User.RequireUniqueEmail = true;
                option.Password.RequireDigit = true;
                option.Password.RequireLowercase = true;
                option.Password.RequireUppercase = true;
                option.Password.RequireNonAlphanumeric = true;
                option.Password.RequiredLength = 8;
                option.Password.RequiredUniqueChars = 8;
                option.User.AllowedUserNameCharacters = "zaqxswcdevfrbgtnhymjukilopZAQXSWCDEVFRBGTNHYMJUKILOP1234567890";

            }).AddRoles<IdentityRole>().AddEntityFrameworkStores<PSDBContext>();
            

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                            .AddJwtBearer(options =>
							{
                                //options.SaveToken = true;
                                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                                {
                                    RequireExpirationTime = true,
                                    ValidateIssuer = true,
                                    ValidateLifetime = true,
                                    ValidateIssuerSigningKey = true,
                                    ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
                                    ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
									IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Key").Value))
								};

								options.Events = new JwtBearerEvents
								{

									OnMessageReceived = (context) =>
									{
										var accessToken = context.Request.Query["access_token"];

										// If the request is for our hub...
										var path = context.HttpContext.Request.Path;
										if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/datasync")))
										{

											context.Token = accessToken;


										}
										return Task.CompletedTask;
									}
								};

							});

			builder.Services.AddMvc();

            var app = builder.Build();
			app.UseWebSockets();
			app.UseCors(options =>
            {
                options.WithOrigins(["https://localhost:4200", "https://localhost"]);
                options.AllowAnyHeader();
                options.WithMethods("GET", "POST");
				options.AllowCredentials();
            });
			// Configure the HTTP request pipeline.
			app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
			app.MapHub<DataSync>("/datasync");
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
