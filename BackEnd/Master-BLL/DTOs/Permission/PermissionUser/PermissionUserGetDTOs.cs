using Master_BLL.DTOs.Permission.PermissionController;
using Master_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Permission.PermissionUser
{
    public record PermissionUserGetDTOs
        (
        string userId,
        string permissionId,
        string permissionName,
        List<PermissionControllerGetDTOs> controllerActionId
        );

}
