using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Modules
{
    public record GetModulesRoles
    (
         string roleId,
        List<string> moduleId
    );
}
