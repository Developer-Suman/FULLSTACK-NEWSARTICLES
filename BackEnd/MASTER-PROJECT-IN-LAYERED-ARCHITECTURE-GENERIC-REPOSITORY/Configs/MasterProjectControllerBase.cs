using AutoMapper;
using Master_BLL.DTOs.Authentication;
using Master_DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Configs
{
    [Route("api/[controller]")]
    [ApiController]
    public class MasterProjectControllerBase : ControllerBase
    {
        public UserDTOs? _currentUser;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        public MasterProjectControllerBase(IMapper mapper, UserManager<ApplicationUser> userManager,  RoleManager<IdentityRole> roleManager) 
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;

        }

        public string ControllerName => this.GetType().Name;

        protected async Task<UserDTOs> GetCurrentUserFromDB()
        {
            if (_currentUser is null)
            {
                var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if(string.IsNullOrEmpty(nameIdentifier))
                {
                    throw new InvalidDataException("Current user not Found");

                }

                var dbUser = _userManager.Users.FirstOrDefault(x=>x.Id == nameIdentifier);
                _currentUser = _mapper.Map<UserDTOs>(dbUser);

            }

            return _currentUser;
        }

        protected IActionResult HandleFailureResult(IEnumerable<string> errors)
        {
            // Check the error messages and return appropriate status code


            if (errors.Any(errors => errors.Contains("Unauthorized")))
            {
                return Unauthorized(errors);
            }
            else if (errors.Any(errors => errors.Contains("NotFound")))
            {
                return NotFound(errors);
            }
            else if (errors.Any(errors => errors.Contains("InsufficientFunds")))
            {
                return StatusCode(402, errors);
            }
            else if (errors.Any(errors => errors.Contains("ForbiddenAccess")))
            {
                return Forbid(string.Join(", ", errors));
            }
            else if (errors.Any(errors => errors.Contains("Conflict")))
            {
                return Conflict(errors);
            }
            else if (errors.Any(errors => errors.Contains("NoContent")))
            {
                return StatusCode(204, new { Message = "No content available.", Errors = errors });
            }
            else
            {
                return BadRequest(errors);
            }
        }

    }
}
