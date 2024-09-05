using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.RegistrationDTOs
{
    public record RegistrationCreateDTOs(
        string Username,
        string Email,
        string Password,
        string Role
        );
}
