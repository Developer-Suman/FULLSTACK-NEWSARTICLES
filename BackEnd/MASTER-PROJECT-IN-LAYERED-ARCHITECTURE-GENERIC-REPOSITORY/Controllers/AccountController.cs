using AutoMapper;
using Master_BLL.DTOs.Authentication;
using Master_BLL.DTOs.RegistrationDTOs;
using Master_BLL.Services.Interface;
using Master_DAL.Abstraction;
using Master_DAL.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Drawing.Printing;
using System.Transactions;

namespace MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Controllers
{
    [Route("api/[controller]"), ApiController, EnableCors("AllowAllOrigins")]
  
    public class AccountController : ControllerBase
    {
        public readonly IAuthenticationRepository _authenticationRepository;
 
        private readonly IMapper _mapper; 
        private readonly IAccountServices _accountServices;
        private readonly IJwtProvider _jwtProvider;

        public AccountController(IJwtProvider jwtProvider,IAccountServices accountServices ,IAuthenticationRepository authenticationRepository,IMapper mapper)
        {
            _authenticationRepository = authenticationRepository;
            _jwtProvider    = jwtProvider;
            _mapper=  mapper;
            _accountServices = accountServices;


        }

        #region Authentication
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegistrationCreateDTOs registrationCreateDTOs)
        {
            var registrationResult = await _accountServices.RegisterUser(registrationCreateDTOs);
            if(registrationResult.Data is not null)
            {
                return Ok(registrationResult.Data);
            }
            else
            {
                return BadRequest(registrationResult.Errors);
            }
            

        }
        #endregion

        #region Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]LoginDTOs userModel)
        {
            var loginResult = await _accountServices.LoginUser(userModel);
            if(loginResult.Data is not null)
            {
                return Ok(loginResult.Data);
            }
            else
            {
                return Unauthorized(loginResult.Errors);
            }
            
        }
        #endregion


        #region Create Roles
        [HttpGet("CreateRole")]
        public async Task<ActionResult> CreateRolesAsync(string rolename)
        {
            var roleExists = await _authenticationRepository.CheckRoleAsync(rolename);
            if(!roleExists)
            {
                var result = await _authenticationRepository.CreateRoles(rolename);
                if(result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            else
            {
                return Conflict("Role Already Exists");
            }

        }
        #endregion

        #region Assign Roles
        [HttpPost("AssignRoles")]
        public async Task<ActionResult> AssignRolesAsync(string userId, string rolename)
        {
            var user = await _authenticationRepository.FindByIdAsync(userId);
            if(user is not null)
            {
                var result = await _authenticationRepository.AssignRoles(user,rolename);
                if(result.Succeeded)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest(result.Errors);
                }

            }
            else
            {
                return NotFound("User not Found");
            }
        }
        #endregion

        #region RefreshToken
        [HttpPost("RefreshToken")]
        public async Task<IActionResult> NewRefreshToken(string refreshtoken,string token)
        {
            var principal = _jwtProvider.GetPrincipalFromExpiredToken(token);
            if(principal is null)
            {
                return Unauthorized("Invalid Token");

            }

            string username = principal.Identity!.Name!;
            if(username is null)
            {
                return Unauthorized("Invalid Token");
            }

            var user = await _authenticationRepository.FindByNameAsync(username);

            if(user is null || user.RefreshToken != refreshtoken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return Unauthorized("Invalid Access Token and refresh Token");
            }


            var roles = await _authenticationRepository.GetRolesAsync(user);
            var newToken = _jwtProvider.Generate(user, roles);
            var newRefreshToken = _jwtProvider.GenerateRefreshToken();
            user.RefreshToken= newToken;
            await _authenticationRepository.UpdateUserAsync(user);

            return Ok(new {Token = newToken, RefreshToken = newRefreshToken });
        }
        #endregion

        #region AllUsers
        [HttpGet("AllUsers")]
        public async Task<IActionResult> GetAllUsers(int page, int pageSize)
        {
            var users = await _authenticationRepository.GetAllUsers(page,pageSize);
            if(users is null)
            {
                return NotFound("User are Not Found");
            }
            return Ok(users);
        }

        #endregion


        #region GetById
        [HttpGet("GetByUserId")]
        public async Task<IActionResult> GetByUserId(string Id)
        {
            var user = await _authenticationRepository.GetById(Id);
            if(user is null)
            {
                return NotFound("User are Not Found");
            }
            return Ok(user);

        }

        #endregion
    }
}
