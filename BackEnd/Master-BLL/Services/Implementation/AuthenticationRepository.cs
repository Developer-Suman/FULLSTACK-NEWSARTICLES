﻿using AutoMapper;
using ExtensionMethods.Pagination;
using Master_BLL.DTOs.Authentication;
using Master_BLL.DTOs.Pagination;
using Master_BLL.Services.Interface;
using Master_BLL.Static.Cache;
using Master_DAL.Abstraction;
using Master_DAL.Interface;
using Master_DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Implementation
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMemoryCacheRepository _memoryCacheRepository;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AuthenticationRepository(UserManager<ApplicationUser> userManager,IMapper mapper, RoleManager<IdentityRole> roleManager, IMemoryCacheRepository memoryCacheRepository,IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _memoryCacheRepository = memoryCacheRepository;
        }

        public async Task<IdentityResult> AssignRoles(ApplicationUser user, string rolename)
        {
            return await _userManager.AddToRoleAsync(user, rolename);
        }

        public Task<IdentityResult> ChangePassword(ApplicationUser user, string currentPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CheckPasswordAsync(ApplicationUser username, string password)
        {
            return await _userManager.CheckPasswordAsync(username, password);
        }

        public async Task<bool> CheckRoleAsync(string role)
        {
            return await _roleManager.RoleExistsAsync(role);
        }

      

        public async Task<IdentityResult> CreateRoles(string role)
        {
            return await _roleManager.CreateAsync(new IdentityRole(role));
            
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user is null)
            {
                return default!;
            }

            return user;
        }

        public async Task<ApplicationUser?> FindByIdAsync(string Id)
        {
            return await _userManager.FindByIdAsync(Id);
        }

        public async Task<ApplicationUser> FindByNameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if(user is null )
            {
                return default!;
            }
            return user;
        }

        public async Task<Result<PagedResult<RoleDTOs>>> GetAllRolesAsync(PaginationDTOs paginationDTOs, CancellationToken cancellationToken)
        {
            var roles = await _unitOfWork.Repository<IdentityRole>().GetAllAsyncWithPagination();
            var rolesPagedResult = await roles.AsNoTracking().ToPagedResultAsync(paginationDTOs.pageIndex, paginationDTOs.pageSize, paginationDTOs.IsPagination);

            var rolesDTOs = _mapper.Map<PagedResult<RoleDTOs>>(rolesPagedResult.Data);

            return Result<PagedResult<RoleDTOs>>.Success(rolesDTOs);
        }

        public async Task<List<UserDTOs>?> GetAllUsers(int page, int pageSize, CancellationToken cancellationToken)
        {
            var cacheKeys = CacheKeys.User;
            var cacheData = await _memoryCacheRepository.GetCahceKey<List<UserDTOs>>(cacheKeys);

            if(cacheData is not null && cacheData.Count > 0)
            {
                return cacheData;
            }
            var users =  await _userManager.Users.AsNoTracking().OrderByDescending(x=>x.CreatedAt)
                .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            await _memoryCacheRepository.SetAsync(cacheKeys, users, new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30) 
            }, cancellationToken);

            var userDTO = _mapper.Map<List<UserDTOs>>(users);

            return userDTO;
        }

        public async Task<Result<PagedResult<UserDTOs>>> GetAllUsersAsync(PaginationDTOs paginationDTOs, CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.Repository<ApplicationUser>().GetAllAsyncWithPagination();
            var usersPagedResult = await users.AsNoTracking().ToPagedResultAsync(paginationDTOs.pageIndex, paginationDTOs.pageSize, paginationDTOs.IsPagination);

            var userDTOs = _mapper.Map<PagedResult<UserDTOs>>(usersPagedResult.Data);

            return Result<PagedResult<UserDTOs>>.Success(userDTOs);
        }

        #region usingIQueryable
        //public async Task<IQueryable<UserDTOs>?> GetAllUsers(int page, int pageSize)
        //{
        //    var cacheKeys = CacheKeys.User;
        //    var cacheData = await _memoryCacheRepository.GetCahceKey<IEnumerable<UserDTOs>>(cacheKeys);

        //    if (cacheData is not null && cacheData.Any())
        //    {
        //        // If data is found in the cache, return an IQueryable from the cached data
        //        return cacheData.AsQueryable();
        //    }

        //    var users = await _userManager.Users.AsNoTracking()
        //        .OrderByDescending(x => x.CreatedAt)
        //        .Skip((page - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    var userDTOs = _mapper.Map<IEnumerable<UserDTOs>>(users);

        //    // Cache the data itself
        //    await _memoryCacheRepository.SetAsync(cacheKeys, userDTOs, new MemoryCacheEntryOptions
        //    {
        //        AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(30)
        //    });

        //    // Return an IQueryable from the cached data
        //    return userDTOs.AsQueryable();
        //}

        #endregion

        public async Task<UserDTOs> GetById(string id, CancellationToken cancellationToken)
        {
            var caheKey = $"GetById{id}";
            var cahceData = await _memoryCacheRepository.GetCahceKey<UserDTOs>(caheKey);
            if(cahceData is not null)
            {
                return cahceData;
            }

            var user = await _userManager.FindByIdAsync(id);
            if(user is null)
            {
                return default!;
            }

            var userDTOs = _mapper.Map<UserDTOs>(user);
            await _memoryCacheRepository.SetAsync<UserDTOs>(caheKey, userDTOs, new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions
            {
                AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(5)
            }, cancellationToken);
            
            return userDTOs;
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser username)
        {
            return await _userManager.GetRolesAsync(username);
        }

        public Task UpdateUserAsync(ApplicationUser user)
        {
            return _userManager.UpdateAsync(user);
        }
    }
}
