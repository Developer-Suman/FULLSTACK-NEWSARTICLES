using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.SubModules
{
    public record SubModulesCreateDTOs(
        string name,
        string iconUrl,
        string targetUrl,
        string role,
        string rank,
        string moduleId
        );
    
}
