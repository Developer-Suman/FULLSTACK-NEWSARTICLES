﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Authentication
{
    public record RoleDTOs(
       string? Id,
       string? Name,
       string? NormalizedName);
}