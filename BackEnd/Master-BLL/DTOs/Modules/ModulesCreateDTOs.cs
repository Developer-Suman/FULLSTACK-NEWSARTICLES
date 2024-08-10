using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Modules
{
    public record ModulesCreateDTOs(
        string Name,
        string? Role,
        string? TargetUrl
        );
}
