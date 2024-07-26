﻿using Master_BLL.DTOs.Permission.PermissionController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Permission
{
    public record ControllerActionUserGetDTOs
        (
        string userId,
        List<PermissionControllerGetDTOs> permissionController
        );
  
}
