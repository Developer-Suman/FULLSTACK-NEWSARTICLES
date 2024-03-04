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
        private readonly IMapper _mapper;
        public MasterProjectControllerBase(UserManager<ApplicationUser> userManager, IMapper mapper ) 
        {
            _userManager = userManager;
            _mapper = mapper;

        }

        protected UserDTOs GetCurrentUserFromDB()
        {
            if (_currentUser is null)
            {
                var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if(string.IsNullOrEmpty(nameIdentifier))
                {
                    throw new InvalidDataException("Current user not Found");

                }

                var dbUser = _userManager.Users.FirstOrDefault(x=>x.UserName == nameIdentifier);
                _currentUser = _mapper.Map<UserDTOs>(dbUser);

            }

            return _currentUser;
        }

    }
}
