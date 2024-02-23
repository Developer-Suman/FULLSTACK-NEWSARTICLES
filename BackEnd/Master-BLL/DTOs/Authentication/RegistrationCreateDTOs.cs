using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.RegistrationDTOs
{
    public class RegistrationCreateDTOs
    {
        public string Username { get; set; }
        [Required(ErrorMessage ="Email is Required")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Password is Required")]
        public string Password { get; set; }
        //[DataType(DataType.Password)]
        //[Compare("Password", ErrorMessage ="The password and Confirmation Password donot Match.")]
        //public string ConfirmPassword { get; set; }

        public string Role { get; set; }
        
    }
}
