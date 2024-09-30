using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using DomainModel;
using DataAccessLayerDB;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationLayer
{
	public class JwtTokenManager
	{
		public JwtPayload GetJWTPayload(string jwt)
		{

			string jwtdata = jwt.Split(" ")[1];
			try
			{
				JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
				JwtSecurityToken t = handler.ReadJwtToken(jwtdata);
				return t.Payload;
			}
			catch
			{
				return null;
			}


		}

		public bool GetUserID(string jwt, out Guid userid)
		{
			if(Guid.TryParse(GetJWTPayload(jwt)[ClaimTypes.NameIdentifier].ToString(),out Guid _userid))
			{
				userid = _userid;
				return true;
			}
			else
			{
				userid = Guid.Empty;
				return false;
			}

		}

		public  async Task<string> GetRoleName(string jwt, RoleManager<IdentityRole> _roleManager)
		{
			IdentityRole role = await _roleManager.FindByNameAsync(GetJWTPayload(jwt)[ClaimTypes.Role].ToString());
			if(role != null)
			{
				return role.Name;
			}
			return null;
				
		}
	}
}
