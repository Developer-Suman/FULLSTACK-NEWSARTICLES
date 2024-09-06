using Master_BLL.DTOs.Menu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.DTOs.SubModules
{
    //public record SubModulesGetDTOs(
    //    string Id,
    //    string name,
    //    string? IconUrl,
    //    string? TargetUrl,
    //    string? Role,
    //    string? Rank,
    //    List<MenuGetDTOs> MenuGetDTOs
    //    );

    public record SubModulesGetDTOs(
      string Id,
      string name,
      string? IconUrl,
      string? TargetUrl,
      string? Role,
      string? Rank
      //List<MenuGetDTOs> MenuGetDTOs
      );

}
