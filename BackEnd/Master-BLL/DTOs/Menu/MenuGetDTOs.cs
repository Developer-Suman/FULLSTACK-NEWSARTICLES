using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.Menu
{
    public record MenuGetDTOs(
        string moduleId,
        string name,
        string icon,
        string targetUrl,
        string role,
        int? rank
        );
    
}
