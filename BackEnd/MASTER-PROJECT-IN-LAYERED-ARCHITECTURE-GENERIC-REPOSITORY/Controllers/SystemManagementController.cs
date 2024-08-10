using AutoMapper;
using Master_BLL.DTOs.Articles;
using Master_BLL.DTOs.Menu;
using Master_BLL.DTOs.Modules;
using Master_BLL.DTOs.SubModules;
using Master_BLL.Services.Implementation;
using Master_BLL.Services.Interface;
using Master_DAL.Abstraction;
using Master_DAL.Models;
using MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemManagementController : MasterProjectControllerBase
    {
        private readonly IModule _module;
        private readonly ISubModule _subModule;
        private readonly IMenu _menu;
   

        public SystemManagementController(IModule module,ISubModule subModule, IMenu menu,IMemoryCacheRepository memoryCacheRepository, UserManager<ApplicationUser> userManager, IMapper mapper) : base(userManager, mapper)
        {
            _module = module;
            _subModule = subModule;
            _menu = menu;     

        }

        [HttpPost("add-module")]
        public async Task<IActionResult> AddModule([FromBody] ModulesCreateDTOs modulesCreateDTOs)
        {
            var savemoduleResult = await _module.Add(modulesCreateDTOs);
            #region switch
            return savemoduleResult switch
            {
                { IsSuccess: true, Data: not null } => CreatedAtAction(nameof(AddModule), savemoduleResult.Data),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(savemoduleResult.Errors),
                _ => BadRequest("Invalid Some Fields")
            };
            #endregion
        }


        [HttpPost("add-submodule")]
        public async Task<IActionResult> AddSubModule([FromBody] SubModulesCreateDTOs subModulesCreateDTOs)
        {
            var savesubmoduleResult = await _subModule.Add(subModulesCreateDTOs);
            #region switch
            return savesubmoduleResult switch
            {
                { IsSuccess: true, Data: not null } => CreatedAtAction(nameof(AddSubModule), savesubmoduleResult.Data),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(savesubmoduleResult.Errors),
                _ => BadRequest("Invalid Some Fields")
            };
            #endregion
        }


        [HttpPost("add-menu")]
        public async Task<IActionResult> AddMenu([FromBody] MenuCreateDTOs menuCreateDTOs)
        {
            var savemenuResult = await _menu.Add(menuCreateDTOs);
            #region switch
            return savemenuResult switch
            {
                { IsSuccess: true, Data: not null } => CreatedAtAction(nameof(AddMenu), savemenuResult.Data),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(savemenuResult.Errors),
                _ => BadRequest("Invalid Some Fields")
            };
            #endregion
        }


        [HttpGet("module/{Id}")]
        public async Task<IActionResult> GetModuleDetails([FromRoute] string Id)
        {
            var getmodulesDetails = await _module.GetModuleWithDetailsAsync(Id);
            #region switch
            return getmodulesDetails switch
            {
                { IsSuccess: true, Data: not null } => CreatedAtAction(nameof(AddMenu), getmodulesDetails.Data),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(getmodulesDetails.Errors),
                _ => BadRequest("Invalid Some Fields")
            };
            #endregion
        }

    }
}
