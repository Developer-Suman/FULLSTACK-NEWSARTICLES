using Master_BLL.DTOs.Authentication;
using Master_BLL.DTOs.RegistrationDTOs;
using Master_DAL.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Interface
{
    public interface IAccountServices
    {
        Task<Result<RegistrationCreateDTOs>> RegisterUser(RegistrationCreateDTOs userModel);
        Task<Result<TokenDTOs>> LoginUser(LoginDTOs userModel);
        //Task<Result<>>

    }
}
