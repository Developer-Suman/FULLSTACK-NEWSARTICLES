using Master_BLL.Services.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Implementation
{
    public class TokenServices : ITokenServices
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public TokenServices(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
            
        }

        private string GetTokenFromAuthorizationHeaders()
        {
            string authorization = _contextAccessor.HttpContext.Request.Headers.Authorization;
            var token = authorization.Split(" ")[1];
            return token;
        }

        public string GetRoles()
        {
            var token = GetTokenFromAuthorizationHeaders();
            var handler= new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var role = jwt.Claims.FirstOrDefault(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role");
            return role!.Value;


        }

        public string GetUserId()
        {
            var token = GetTokenFromAuthorizationHeaders();
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var uid = jwt.Claims.FirstOrDefault(x => x.Type == "uid");
            return uid!.Value;
        }

        public string GetUsername()
        {
            var token = GetTokenFromAuthorizationHeaders();
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var username = jwt.Claims.FirstOrDefault(x=>x.Type == ClaimTypes.Name);
            return username!.Value;
        }

      

        
    }
}
