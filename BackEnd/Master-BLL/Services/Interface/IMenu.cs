using Master_BLL.DTOs.Menu;
using Master_DAL.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Interface
{
    public interface IMenu
    {
        Task<Result<MenuGetDTOs>> Add(MenuCreateDTOs menuCreateDTOs);
    }
}
