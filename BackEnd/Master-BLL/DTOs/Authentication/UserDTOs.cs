using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Authentication
{
    public record UserDTOs
        (
        string Id,
        string FirstName,
        string LastName,
        string UserName,
        string Address,
        string Email
        );
}
