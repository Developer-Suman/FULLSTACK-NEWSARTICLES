﻿using System;
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
        string permissionName
        );

}
