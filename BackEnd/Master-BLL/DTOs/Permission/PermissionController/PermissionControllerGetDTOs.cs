using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Permission.PermissionController
{
    public record PermissionControllerGetDTOs(
        string controllerActionId,
        string controllerActionName
        );
 
}
