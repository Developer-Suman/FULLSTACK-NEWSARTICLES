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
        Task<Result<ModulesGetDTOs>> GetModuleWithDetails(string ModuleId);
        Task<Result<GetModulesRoles>> AssignModulesToRole(string roleId, IEnumerable<string> moduleIds);
        Task<Result<GetModulesRoles>> RemoveModulesFromRole(string roleId, IEnumerable<string> moduleIds);
        Task<Result<List<ModulesGetDTOs>>> GetNavigationMenuByUser(string userId);
    }
}
