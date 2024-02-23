using AutoMapper;
using Master_BLL.DTOs.Authentication;
using Master_BLL.DTOs.RegistrationDTOs;
using Master_BLL.Services.Interface;
using Master_DAL.Abstraction;
using Master_DAL.JWT;
using Master_DAL.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Master_BLL.Services.Implementation
{
    public class AccountServices : IAccountServices
    {
        private readonly IMapper _mapper;
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IJwtProvider _jwtProvider;
        private readonly IConfiguration _configuration;

        public AccountServices(IMapper mapper, IAuthenticationRepository authenticationRepository, IJwtProvider jwtProvider, IConfiguration configuration)
        {
            _mapper = mapper;
            _jwtProvider = jwtProvider;
            _authenticationRepository = authenticationRepository;
            _configuration = configuration;
        }

        public async Task<Result<TokenDTOs>> LoginUser(LoginDTOs userModel)
        {
            try
            {
                var user = await _authenticationRepository.FindByEmailAsync(userModel.Email);
                if (user is null)
                {
                    return Result<TokenDTOs>.Failure("Unauthorized: Invalid Credentials");
                }
                if (!await _authenticationRepository.CheckPasswordAsync(user, userModel.Password))
                {
                    return Result<TokenDTOs>.Failure("Uauthorized:Invalid Credentials");

                }

                var roles = await _authenticationRepository.GetRolesAsync(user);
                string token = _jwtProvider.Generate(user, roles);

                string refreshToken = _jwtProvider.GenerateRefreshToken();

                user.RefreshToken = refreshToken;

                _ = int.TryParse(_configuration["Jwt:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

                await _authenticationRepository.UpdateUserAsync(user);
                var data = new TokenDTOs()
                {
                    Token = token,
                    RefreshToken = refreshToken,

                };
                //return Ok(new { Token = token, RefreshToken = refreshToken });
                return Result<TokenDTOs>.Success(data);

            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while loggig");
            }
        }

        public async Task<Result<RegistrationCreateDTOs>> RegisterUser(RegistrationCreateDTOs userModel)
        {
            using(var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var userExists = await _authenticationRepository.FindByNameAsync(userModel.Username);
                    if (userExists is not null)
                    {
                        return Result<RegistrationCreateDTOs>.Failure("User already Exists");
                    }

                    var emailExists = await _authenticationRepository.FindByEmailAsync(userModel.Email);
                    if (emailExists is not null)
                    {
                        return Result<RegistrationCreateDTOs>.Failure("Email Already Exisyts");
                    }

                    var user = _mapper.Map<ApplicationUser>(userModel);

                    //ApplicationUser user = new()
                    //{
                    //    UserName = registrationCreateDTOs.Username,
                    //    Email = registrationCreateDTOs.Email,
                    //    SecurityStamp = Guid.NewGuid().ToString(),
                    //};

                    var result = await _authenticationRepository.CreateUserAsync(user, userModel.Password);

                    if (!result.Succeeded)
                    {
                        scope.Dispose();
                        return Result<RegistrationCreateDTOs>.Failure("User Creation Failed");
                    }

                    //Add the user to desired role
                    if (!string.IsNullOrWhiteSpace(userModel.Role))
                    {
                         await _authenticationRepository.AssignRoles(user, userModel.Role);

                    }
                    
                    //if everything succeed, commit the transaction
                    scope.Complete();

                    var userDisplay  = _mapper.Map<RegistrationCreateDTOs>(userModel);

                    return Result<RegistrationCreateDTOs>.Success(userDisplay);

                }
                catch (Exception ex)
                {
                    throw new Exception("Something went wrong during user creation");
                }

            }
        }
    }
}
