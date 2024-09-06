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

        public async Task<Result<AssignRolesDTOs>> AssignRoles(AssignRolesDTOs assignRolesDTOs)
        {
            try
            {
                var user = await _authenticationRepository.FindByIdAsync(assignRolesDTOs.UserId);
                if (user is null)
                {
                    return Result<AssignRolesDTOs>.Failure("NotFound", "User not found to assign roles");
                }

                var result = await _authenticationRepository.AssignRoles(user, assignRolesDTOs.RoleName);
                return Result<AssignRolesDTOs>.Success(assignRolesDTOs);

            }
            catch (Exception ex)
            {
                throw new Exception("Failed to Assign Roles");
            }
        }

        public async Task<Result<ChangePasswordDTOs>> ChangePassword(string userId, ChangePasswordDTOs changePasswordDTOs)
        {
            try
            {
                var user = await _authenticationRepository.FindByIdAsync(userId);
                if(user is null)
                {
                    return Result<ChangePasswordDTOs>.Failure("NotFound", "User is not found");
                }

                var changePasswordResult = await _authenticationRepository.ChangePassword(user, changePasswordDTOs.CurrentPassword!, changePasswordDTOs.NewPassword!);

                var chanagePasswordDTOs =new ChangePasswordDTOs(changePasswordDTOs.CurrentPassword, changePasswordDTOs.NewPassword);
                return Result<ChangePasswordDTOs>.Success(changePasswordDTOs);


            }
            catch (Exception ex)
            {
                throw new Exception("Failed to change Password");
            }
        }

        public async Task<Result<string>> CreateRoles(string rolename)
        {
            try
            {
                var roleExists = await _authenticationRepository.CheckRoleAsync(rolename);
                if(roleExists)
                {
                    return Result<string>.Failure("Conflict", "Roles Already Exists");
                }

                var result = await _authenticationRepository.CreateRoles(rolename);
                return Result<string>.Success(rolename);

            }
            catch (Exception ex)
            {
                throw new Exception("An error occured whle creating roles");
            }
        }

        public async Task<Result<UserDTOs>> GetByUserId(string userId, CancellationToken cancellationToken)
        {
            var user = await _authenticationRepository.GetById(userId, cancellationToken);
            if (user is null)
            {
                return Result<UserDTOs>.Failure("NotFound", "User are Not Found");
            }
            return Result<UserDTOs>.Success(user);
        }

        public async Task<Result<TokenDTOs>> GetNewToken(TokenDTOs tokenDTOs)
        {
            try
            {
                var principal = _jwtProvider.GetPrincipalFromExpiredToken(tokenDTOs.Token);
                if (principal is null)
                {
                    return Result<TokenDTOs>.Failure("Unauthorized", "Invalid Token");
                }

                string username = principal.Identity!.Name!;
                if (username is null)
                {
                    return Result<TokenDTOs>.Failure("Unauthorized", "Invalid Token");
                }

                var user = await _authenticationRepository.FindByNameAsync(username);

                if (user is null || user.RefreshToken != tokenDTOs.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
                {
                    return Result<TokenDTOs>.Failure("Unauthorized", "Invalid Access Token and refresh Token");
                }

                var roles = await _authenticationRepository.GetRolesAsync(user);
                var newToken = _jwtProvider.Generate(user, roles);

                var newRefreshToken = _jwtProvider.GenerateRefreshToken();
                user.RefreshToken = newRefreshToken;
                await _authenticationRepository.UpdateUserAsync(user);

                tokenDTOs = new TokenDTOs(newToken, newRefreshToken);
                return Result<TokenDTOs>.Success(tokenDTOs);

            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while getting new Token");
            }
        }

        public async Task<Result<TokenDTOs>> LoginUser(LoginDTOs userModel)
        {
            try
            {
                var user = await _authenticationRepository.FindByEmailAsync(userModel.Email);
                if (user is null)
                {
                    return Result<TokenDTOs>.Failure("Unauthorized", "Invalid Credentials");
                }
                if (!await _authenticationRepository.CheckPasswordAsync(user, userModel.Password))
                {
                    return Result<TokenDTOs>.Failure("Unauthorized", "Invalid Password");

                }

                var roles = await _authenticationRepository.GetRolesAsync(user);
                if (roles is null)
                {
                    return Result<TokenDTOs>.Failure("NotFound", "Roles are not found");
                }

                string token = _jwtProvider.Generate(user, roles);

                string refreshToken = _jwtProvider.GenerateRefreshToken();

                user.RefreshToken = refreshToken;

                _ = int.TryParse(_configuration["Jwt:RefreshTokenValidityInDays"], out int refreshTokenValidityInDays);
                user.RefreshTokenExpiryTime = DateTime.Now.AddDays(refreshTokenValidityInDays);

                await _authenticationRepository.UpdateUserAsync(user);
                var logInToken = new TokenDTOs(token, refreshToken);
                return Result<TokenDTOs>.Success(logInToken);

            }
            catch (Exception ex)
            {
                throw new Exception("Something went wrong while loggig");
            }
        }

        public async Task<Result<object>> LogoutUser(string userId)
        {
            try
            {
                var user = await _authenticationRepository.FindByIdAsync(userId);
                if (user is null)
                {
                    return Result<object>.Failure("NotFound", "User is not Found");
                }

                user.RefreshToken = null;
                await _authenticationRepository.UpdateUserAsync(user);
                return Result<object>.Success(true);

            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while LogOut Users");
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
                        return Result<RegistrationCreateDTOs>.Failure("Conflict", "User Already Exists");
                    }

                    var emailExists = await _authenticationRepository.FindByEmailAsync(userModel.Email);
                    if (emailExists is not null)
                    {
                        return Result<RegistrationCreateDTOs>.Failure("Conflict", "Email Already Exists");
                    }

                    var user = _mapper.Map<ApplicationUser>(userModel);

                    var result = await _authenticationRepository.CreateUserAsync(user, userModel.Password);

                    if (!result.Succeeded)
                    {
                        scope.Dispose();
                        return Result<RegistrationCreateDTOs>.Failure("Conflict", "User Creation Failed");
                    }

                    //Add User to Desired Role
                    if (!string.IsNullOrEmpty(userModel.Role))
                    {
                        await _authenticationRepository.AssignRoles(user, userModel.Role);
                    }

                    //if everything succeed, commit the transaction
                    scope.Complete();

                    //If Everything succeed Commit the transaction
                    scope.Complete();

                    var userDisplay = new RegistrationCreateDTOs(
                        userModel.Username,
                        userModel.Email,
                        userModel.Password,
                        userModel.Role
                        );

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
