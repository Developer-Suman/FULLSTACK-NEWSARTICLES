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
        Task<Result<TokenDTOs>> LoginUser(LogInDTOs userModel);
        Task<Result<object>> LogoutUser(string userId);
        Task<Result<ChangePasswordDTOs>> ChangePassword(string userId, ChangePasswordDTOs changePasswordDTOs);
        Task<Result<string>> CreateRoles(string rolename);
        Task<Result<AssignRolesDTOs>> AssignRoles(AssignRolesDTOs assignRolesDTOs);
        Task<Result<TokenDTOs>> GetNewToken(TokenDTOs tokenDTOs);
        //Task<Result<PagedResult<RoleDTOs>>> GetAllRoles(PaginationDTOs paginationDTOs, CancellationToken cancellationToken);
        //Task<Result<PagedResult<UserDTOs>>> GetAllUsers(PaginationDTOs paginationDTOs, CancellationToken cancellationToken);
        Task<Result<UserDTOs>> GetByUserId(string userId, CancellationToken cancellationToken);

    }
}
