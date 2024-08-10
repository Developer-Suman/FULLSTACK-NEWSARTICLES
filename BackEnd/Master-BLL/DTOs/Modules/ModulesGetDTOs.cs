using Master_BLL.DTOs.SubModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Modules
{
    public record ModulesGetDTOs(
        string Id,
        string Name,
        string? Role,
        string? TargetUrl,
        List<SubModulesGetDTOs> SubModulesGetDTOs
        );
   
}
