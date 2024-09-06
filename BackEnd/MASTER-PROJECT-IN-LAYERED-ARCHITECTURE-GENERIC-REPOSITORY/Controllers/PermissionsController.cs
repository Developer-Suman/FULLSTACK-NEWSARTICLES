using AutoMapper;
using Master_BLL.DTOs.Permission;
using Master_BLL.DTOs.Permission.PermissionController;
using Master_BLL.DTOs.Permission.PermissionUser;
using Master_BLL.Services.Interface;
using Master_DAL.Models;
using MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionsController : MasterProjectControllerBase
    {
        private readonly IPermissionServices _permissionServices;


        public PermissionsController(IPermissionServices permissionServices, IMemoryCacheRepository memoryCacheRepository, UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager) : base(userManager, mapper, roleManager)
        {
            _permissionServices = permissionServices;

        }

        [HttpPost("AssignPermission")]
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

        [HttpGet("GetAllUser")]
        public async Task<IActionResult> GetAllUser()
        {
            var getAllUser = await _permissionServices.GetAllUser();

            #region switch
            return getAllUser switch
            {
                { IsSuccess: true, Data: not null } => new JsonResult(getAllUser.Data, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                }),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(getAllUser.Errors),
                _ => BadRequest("Invalid Data")
            };
            #endregion
        }


        [HttpGet("GetPermissionByUserId/{userId}")]
        public async Task<IActionResult> GetPermissionByUserId([FromRoute] string userId)
        {
            var getPermission = await _permissionServices.GetPermissionByUserId(userId);

            #region switch
            return getPermission switch
            {
                { IsSuccess: true, Data: not null } => CreatedAtAction(nameof(AssignPermission), getPermission.Data),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(getPermission.Errors),
                _ => BadRequest("Invalid Some Fields")
            };
            #endregion
        }

        [HttpGet("GetControllerActionByUserId/{UserId}")]
        public async Task<IActionResult> GetControllerActionByUserId([FromRoute] string UserId)
        {
            var getControllerAction = await _permissionServices.GetControllerActionByUserId(UserId);

            #region switch
            return getControllerAction switch
            {
                { IsSuccess: true, Data: not null } => CreatedAtAction(nameof(AssignPermission), getControllerAction.Data),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(getControllerAction.Errors),
                _ => BadRequest("Invalid Some Fields")
            };
            #endregion
        }

        [HttpPost("AssignTaskToListOfUsers")]
        public async Task<IActionResult> AssignTaskToListOfUsers([FromBody] List<PermissionGetDTOs> permissionGetDTOs)
        {
            var assignTask = await _permissionServices.AssignTaskToListOfUsers(permissionGetDTOs);

            #region switch
            return assignTask switch
            {
                { IsSuccess: true, Data: not null } => CreatedAtAction(nameof(AssignPermission), assignTask.Data),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(assignTask.Errors),
                _ => BadRequest("Invalid Some Fields")
            };
            #endregion
        }
    }

}
