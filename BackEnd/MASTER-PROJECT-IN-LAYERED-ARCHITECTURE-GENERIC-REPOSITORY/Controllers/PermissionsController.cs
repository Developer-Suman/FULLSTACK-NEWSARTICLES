using AutoMapper;
using Master_BLL.DTOs.Permission;
using Master_BLL.Services.Interface;
using Master_DAL.Models;
using MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : MasterProjectControllerBase
    {
        private readonly IPermissionServices _permissionServices;


        public PermissionsController(IPermissionServices permissionServices, IMemoryCacheRepository memoryCacheRepository, UserManager<ApplicationUser> userManager, IMapper mapper) : base(userManager, mapper)
        {
            _permissionServices = permissionServices;

        }

        [HttpPost]
        public async Task<IActionResult> AssignPermission([FromBody] PermissionDTOs permissionDTOs)
        {
            var permissionResult = await _permissionServices.AssignControllerActionsToPermissionsAsync(permissionDTOs);

            #region switch
            return permissionResult switch
            {
                { IsSuccess: true, Data: not null } => CreatedAtAction(nameof(AssignPermission), permissionResult.Data),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(permissionResult.Errors),
                _ => BadRequest("Invalid Some Fields")
            };
            #endregion
        }

        [HttpPost]
        public async Task<IActionResult> RemovePermission([FromBody] PermissionDTOs permissionDTOs)
        {
            var permissionRemoveResult = await _permissionServices.AssignControllerActionsToPermissionsAsync(permissionDTOs);

            #region switch
            return permissionRemoveResult switch
            {
                { IsSuccess: true, Data: not null } => CreatedAtAction(nameof(RemovePermission), permissionRemoveResult.Data),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(permissionRemoveResult.Errors),
                _ => BadRequest("Invalid Some Fields")
            };
            #endregion
        }


        [HttpPost("AssignPermission")]
        public async Task<IActionResult> AssignPermissionToUser([FromBody] PermissionUserDTOs permissionUserDTOs)
        {
            var permissionUserResult = await _permissionServices.AssignPermissionToUserAsync(permissionUserDTOs);

            #region switch
            return permissionUserResult switch
            {
                { IsSuccess: true, Data: not null } => CreatedAtAction(nameof(AssignPermission), permissionUserResult.Data),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(permissionUserResult.Errors),
                _ => BadRequest("Invalid Some Fields")
            };
            #endregion
        }

        [HttpPost("RemovePermission")]
        public async Task<IActionResult> RemovePermissionToUser([FromBody] PermissionUserDTOs permissionUserDTOs)
        {
            var permissionremoveUserResult = await _permissionServices.RemovePermissionToUserAsync(permissionUserDTOs);

            #region switch
            return permissionremoveUserResult switch
            {
                { IsSuccess: true, Data: not null } => CreatedAtAction(nameof(AssignPermission), permissionremoveUserResult.Data),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(permissionremoveUserResult.Errors),
                _ => BadRequest("Invalid Some Fields")
            };
            #endregion
        }
    }

}
