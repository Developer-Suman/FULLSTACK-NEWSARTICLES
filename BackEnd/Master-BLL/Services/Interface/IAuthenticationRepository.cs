using Master_BLL.DTOs.Authentication;
using Master_BLL.DTOs.Pagination;
using Master_DAL.Abstraction;
using Master_DAL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Interface
{
    public interface IAuthenticationRepository
    {
        Task<ApplicationUser> FindByNameAsync(string username);
        Task<ApplicationUser> FindByIdAsync(string Id);
        Task<IList<string>> GetRolesAsync(ApplicationUser username);
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(ApplicationUser username, string password);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task UpdateUserAsync(ApplicationUser user);
        Task<IdentityResult> CreateRoles(string role);
        Task<IdentityResult> AssignRoles(ApplicationUser user, string rolename);
        Task<bool> CheckRoleAsync(string role);
        Task<IdentityResult> ChangePassword(ApplicationUser user, string currentPassword, string newPassword);
        Task<List<UserDTOs>?> GetAllUsers(int page, int pageSize, CancellationToken cancellationToken);
        Task<UserDTOs> GetById(string id,CancellationToken cancellationToken);
        Task<Result<PagedResult<RoleDTOs>>> GetAllRolesAsync(PaginationDTOs paginationDTOs, CancellationToken cancellationToken);
        Task<Result<PagedResult<UserDTOs>>> GetAllUsersAsync(PaginationDTOs paginationDTOs, CancellationToken cancellationToken);
    }
}
