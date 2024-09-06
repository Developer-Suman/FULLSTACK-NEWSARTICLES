using AutoMapper;
using Master_BLL.DTOs.Authentication;
using Master_BLL.DTOs.RegistrationDTOs;
using Master_BLL.Services.Interface;
using Master_DAL.Abstraction;
using Master_DAL.Models;
using MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Configs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Drawing.Printing;
using System.Text.Json;
using System.Transactions;

namespace MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Controllers
{
    [Route("api/[controller]"), ApiController, EnableCors("AllowAllOrigins")]
  
    public class AccountController : MasterProjectControllerBase
    {
        public readonly IAuthenticationRepository _authenticationRepository;
 
        private readonly IAccountServices _accountServices;
        private readonly IJwtProvider _jwtProvider;

        public AccountController(IJwtProvider jwtProvider,IAccountServices accountServices ,IAuthenticationRepository authenticationRepository, UserManager<ApplicationUser> userManager, IMapper mapper, RoleManager<IdentityRole> roleManager) : base(userManager, mapper,  roleManager)
        {
            _authenticationRepository = authenticationRepository;
            _jwtProvider    = jwtProvider;
            _accountServices = accountServices;


        }

        #region Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegistrationCreateDTOs registrationCreateDTOs)
        {
            var registrationResult = await _accountServices.RegisterUser(registrationCreateDTOs);

            #region switch Statement
            return registrationResult switch
            {
                { IsSuccess: true, Data: not null } => CreatedAtAction(nameof(Register), registrationResult.Data),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(registrationResult.Errors),
                _ => BadRequest("Invalid Fields for Register User")
            };
            #endregion


        }
        #endregion

        #region Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]LogInDTOs userModel)
        {
            var logInResult = await _accountServices.LoginUser(userModel);

            #region Switch Statement
            return logInResult switch
            {
                { IsSuccess: true, Data: not null } => new JsonResult(logInResult.Data, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                }),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(logInResult.Errors),
                _ => BadRequest("Invalid Username and Password")
            };

            #endregion

        }
        #endregion


        #region Create Roles
        [HttpPost("CreateRoles")]
        public async Task<IActionResult> CreateRoles([FromQuery] string rolename)
        {
            var roleResult = await _accountServices.CreateRoles(rolename);
            #region switch Statement
            return roleResult switch
            {
                { IsSuccess: true, Data: not null } => CreatedAtAction(nameof(CreateRoles), roleResult.Data),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(roleResult.Errors),
                _ => BadRequest("Invalid rolename Fields")
            };
            #endregion

        }
        #endregion

        #region Assign Roles
        [HttpPost("AssignRoles")]
        public async Task<IActionResult> AssignRolesAsync([FromBody] AssignRolesDTOs assignRolesDTOs)
        {
            var assignRolesResult = await _accountServices.AssignRoles(assignRolesDTOs);
            #region switch Statement
            return assignRolesResult switch
            {
                { IsSuccess: true, Data: not null } => new JsonResult(assignRolesResult.Data, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                }),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(assignRolesResult.Errors),
                _ => BadRequest("Invalid rolename and userId Fields ")
            };
            #endregion
        }
        #endregion

        #region RefreshToken
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> NewRefreshToken([FromBody] TokenDTOs tokenDTOs)
        {
            var getNewTokenResult = await _accountServices.GetNewToken(tokenDTOs);

            #region Switch Statement
            return getNewTokenResult switch
            {
                { IsSuccess: true, Data: not null } => new JsonResult(getNewTokenResult.Data, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                }),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(getNewTokenResult.Errors),
                _ => BadRequest("Invalid accesstoken and refreshtoken Fields ")
            };
            #endregion
        }
        #endregion

        #region AllUsers
        //[HttpGet("AllUsers")]
        //public async Task<IActionResult> GetAllUsers(int page, int pageSize, CancellationToken cancellationToken)
        //{
        //    var getAllUserResult = await _accountServices.GetAllUsers(paginationDTOs, cancellationToken);
        //    #region Switch Statement
        //    return getAllUserResult switch
        //    {
        //        { IsSuccess: true, Data: not null } => new JsonResult(getAllUserResult.Data, new JsonSerializerOptions
        //        {
        //            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        //        }),
        //        { IsSuccess: false, Errors: not null } => HandleFailureResult(getAllUserResult.Errors),
        //        _ => BadRequest("Invalid page and pageSize Fields ")
        //    };
        //    #endregion
        //}

        #endregion



        #region GetByUserId
        [HttpGet("GetByUserId")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        //[Authorize(Roles ="admin")]
        public async Task<IActionResult> GetByUserId(string Id, CancellationToken cancellationToken)
        {
            var getbyUserIdResult = await _accountServices.GetByUserId(Id, cancellationToken);

            #region Switch Statement
            return getbyUserIdResult switch
            {
                { IsSuccess: true, Data: not null } => new JsonResult(getbyUserIdResult.Data, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                }),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(getbyUserIdResult.Errors),
                _ => BadRequest("Invalid page and pageSize Fields ")
            };
            #endregion
        }
        #endregion

        #region GetAllRoles
        //[HttpGet("GetAllRoles")]
        //public async Task<IActionResult> GetAllUserRoles([FromQuery] PaginationDTOs paginationDTOs, CancellationToken cancellationToken)
        //{
        //    var getAllUserRolesResult = await _accountServices.GetAllRoles(paginationDTOs, cancellationToken);
        //    #region Switch Statement
        //    return getAllUserRolesResult switch
        //    {
        //        { IsSuccess: true, Data: not null } => new JsonResult(getAllUserRolesResult.Data, new JsonSerializerOptions
        //        {
        //            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        //        }),
        //        { IsSuccess: false, Errors: not null } => HandleFailureResult(getAllUserRolesResult.Errors),
        //        _ => BadRequest("Invalid page and pageSize Fields ")
        //    };
        //    #endregion

        //}
        #endregion

        #region LogOutUser
        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("LogOut")]
        public async Task<IActionResult> logOut()
        {
            await GetCurrentUser();
            var LogOutResult = await _accountServices.LogoutUser(_currentUser!.Id.ToString());
            #region Switch Statement
            return LogOutResult switch
            {
                { IsSuccess: true, Data: not null } => new JsonResult(LogOutResult.Data, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                }),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(LogOutResult.Errors),
                _ => BadRequest("Invalid page and pageSize Fields ")
            };
            #endregion
        }

        #endregion

        #region ChangePassword
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTOs changePasswordDTOs)
        {
            await GetCurrentUser();
            var changePasswordResult = await _accountServices.ChangePassword(_currentUser!.Id.ToString(), changePasswordDTOs);
            #region Switch Statement
            return changePasswordResult switch
            {
                { IsSuccess: true, Data: not null } => new JsonResult(changePasswordResult.Data, new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                }),
                { IsSuccess: false, Errors: not null } => HandleFailureResult(changePasswordResult.Errors),
                _ => BadRequest("Invalid page and pageSize Fields ")
            };
            #endregion
        }
        #endregion
    }
}
