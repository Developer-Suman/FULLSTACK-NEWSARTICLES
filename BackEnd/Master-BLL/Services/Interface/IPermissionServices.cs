using Master_BLL.DTOs.Permission;
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

        Task<Result<bool>> RemoveControllerActionsToPermissionsAsync(PermissionDTOs permissionDTOs);

        Task<Result<List<PermissionAuthorizedDTOs>>> GetPermissionAuthorizedData(string CurrentUser);

        Task<Result<AssignPermissionGetDTOs>> AssignPermissionToUserAsync(string userId, List<string> permissionIds);
        Task<Result<AssignPermissionGetDTOs>> RemovePermissionToUserAsync(string userId, List<string> permissionIds);

        Task<Result<AssignControllerActionToUserGetDTOs>> AssignControllerActionToUserAsync(string userId, List<string> controlleractionId);
        Task<Result<AssignControllerActionToUserGetDTOs>> RemoveControllerActionToUserAsync(string userId, List<string> controlleractionId);


    }
}
