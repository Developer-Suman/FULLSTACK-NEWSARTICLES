using Master_BLL.DTOs.Authentication;
using Master_BLL.DTOs.Permission;
using Master_BLL.DTOs.Permission.PermissionUser;
using Master_DAL.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Interface
{
    public interface IPermissionServices
    {
        Task<Result<PermissionDTOs>> AssignControllerActionsToPermissionsAsync(PermissionDTOs permissionDTOs);

        Task<Result<List<PermissionAuthorizedDTOs>>> GetPermissionAuthorizedData(string CurrentUser);

        Task<Result<List<UserDTOs>>> GetAllUser();
        Task<Result<List<PermissionUserGetDTOs>>> GetPermissionByUserId(string UserId);

        Task<Result<ControllerActionUserGetDTOs>> GetControllerActionByUserId(string UserId);

        Task<Result<AssignPermissionGetDTOs>> AssignPermissionToUserAsync(PermissionUserDTOs permissionUserDTOs);
        Task<Result<AssignPermissionGetDTOs>> RemovePermissionToUserAsync(PermissionUserDTOs permissionUserDTOs);


    }
}
