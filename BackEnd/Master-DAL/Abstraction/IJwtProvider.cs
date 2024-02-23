using Master_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;                                                      
using System.Threading.Tasks;

namespace Master_DAL.Abstraction
{
    public interface IJwtProvider
    {
        string Generate(ApplicationUser user, IList<string> roles);
        string GenerateRefreshToken();

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
