using Master_BLL.DTOs.Modules;
using Master_DAL.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Interface
{
    public interface IModule
    {
        Task<Result<ModulesGetDTOs>> Add(ModulesCreateDTOs modulesCreateDTOs);
        Task<Result<ModulesGetDTOs>> GetModuleWithDetailsAsync(string ModuleId);
    }
}
