using DataAccessLayerDB;
using AAAService.Core;
using DomainModel;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SignalR
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();

			Log.Logger = new LoggerConfiguration()
				.ReadFrom.Configuration(builder.Configuration)
				.Enrich.FromLogContext()
				.CreateLogger();

			builder.Host.UseSerilog((context, services, configuration) => configuration
										.ReadFrom.Configuration(context.Configuration)
										.ReadFrom.Services(services)
										);

			builder.Services.AddSignalR(options => {
				options.ClientTimeoutInterval = TimeSpan.FromMinutes(5);
				options.MaximumParallelInvocationsPerClient = 2;
				});


			builder.Services.AddCors();
			string connectionstring = string.Empty;
			try
			{
				connectionstring = builder.Configuration.GetConnectionString("sqlConnection");
			}
			catch (Exception ex)
			{
				Log.Fatal("AppSettings.json file is missing OR 'sqlConnection' key is missing from the configuration file - {0}\nDetails:\n{1}\n\n", DateTime.Now, ex.ToString());
			}

			try
			{
				builder.Services.AddDbContext<PSDBContext>(option => option.UseSqlServer(connectionstring));
			}
			catch (Exception ex)
			{
				Log.Fatal("Can NOT connect to the database - {0}\nDetails:\n{1}\n\n", DateTime.Now, ex.ToString());
			}


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

			//builder.Services.AddWebSockets(wsoptions => {
			//    wsoptions.KeepAliveInterval = TimeSpan.FromSeconds(30);
			//    });

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
									IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt:Key").Value))
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


			builder.Services.AddScoped<Validation>();
			builder.Services.AddScoped<JwtManager>();


			var app = builder.Build();
			// Configure the HTTP request pipeline.

			app.UseWebSockets();
			app.UseCors(options =>
			{
				options.WithOrigins(["https://localhost:4200", "https://localhost", "https://localhost:40443"]);
				options.AllowAnyHeader();
				options.WithMethods("GET", "POST");
				options.AllowCredentials();
			});

			app.UseAuthentication();
			app.UseAuthorization();
			

			app.MapHub<DataSync>("/datasync");

			app.Run();
		}
	}
} 
