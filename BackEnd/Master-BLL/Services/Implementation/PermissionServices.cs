using AutoMapper;
using Master_BLL.DTOs.Articles;
using Master_BLL.DTOs.Permission;
using Master_BLL.Services.Interface;
using Master_DAL.Abstraction;
using Master_DAL.DbContext;
using Master_DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL.Services.Implementation
{
    public class PermissionServices : IPermissionServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PermissionServices(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            
        }
        public async Task<Result<PermissionDTOs>> AssignControllerActionsToPermissionsAsync(PermissionDTOs permissionDTOs)
        {
            try
            {
                var userExists = await _context.ApplicationUsers.AnyAsync(u => u.Id == permissionDTOs.userId);
                if (!userExists)
                {
                    return Result<PermissionDTOs>.Failure("NotFound","User not Found");
                    
                }

                var permissions = await _context.Permissions
                    .Where(p=> permissionDTOs.permissionIds.Contains(p.Id)).ToListAsync();

                if (permissions.Count != permissionDTOs.permissionIds.Count())
                {
                    return Result<PermissionDTOs>.Failure("NotFound", "Permissions Don not exists");
                }

                var controllerActions = await _context.ControllerActions
                    .Where(ca => permissionDTOs.controllerActionIds.Contains(ca.Id)).ToListAsync();

                if(controllerActions.Count != permissionDTOs.controllerActionIds.Count())
                {
                    return Result<PermissionDTOs>.Failure("NotFound", "One or more controller actions do not exist.");
                }

                // Create PermissionControllerAction entries
                var permissionControllerActions = permissions
                    .SelectMany(p => controllerActions, (p, ca) => new PermissionControllerAction
                    {
                        PermissionId = p.Id,
                        ControlleractionId = ca.Id
                    });

                // Add the entries to the database
                _context.PermissionControllerActions.AddRange(permissionControllerActions);

                // Assign permissions to the user
                var userPermissions = permissions.Select(p => new UserPermission
                {
                    UserId = permissionDTOs.userId,
                    PermissionId = p.Id
                });

                _context.UserPermissions.AddRange(userPermissions);


                await _context.SaveChangesAsync();

                var result = new PermissionDTOs(
                    permissionDTOs.userId,
                    permissions.Select(x=>x.Id),
                    controllerActions.Select(x=>x.Id)
                    );

            

                return Result<PermissionDTOs>.Success(result);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while Adding permission");
                //return Task.FromResult(false);
            } 
        }

        public Task<Result<AssignControllerActionToUserGetDTOs>> AssignControllerActionToUserAsync(string userId, List<string> controlleractionId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<AssignPermissionGetDTOs>> AssignPermissionToUserAsync(string userId, List<string> permissionIds)
        {
            try
            {
                var permissions = await _context.Permissions
                    .Where(x=> permissionIds.Contains(x.Id)).ToListAsync();

                if(permissions is  null)
                {
                    return Result<AssignPermissionGetDTOs>.Failure("NotFound", "Permission are not found");
                }

                var UserPermission = permissions
                   .Select(p => new UserPermission
                   {
                       UserId = userId,
                       PermissionId = p.Id
                   }).ToList();

                _context.UserPermissions.AddRange(UserPermission);
                await _context.SaveChangesAsync();

              
                var assignedPermissions = UserPermission.Select(up => _mapper.Map<AssignPermissionGetDTOs>(up)).ToList();

                return Result<AssignPermissionGetDTOs>.Success(new AssignPermissionGetDTOs(userId, assignedPermissions));
               


            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while AssigningPermission");
            }
        }

        public Task<Result<List<PermissionAuthorizedDTOs>>> GetPermissionAuthorizedData(string CurrentUser)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<bool>> RemoveControllerActionsToPermissionsAsync(PermissionDTOs permissionDTOs)
        {
            // Check if the user exists
            var userExists = await _context.ApplicationUsers.AnyAsync(u => u.Id == permissionDTOs.userId);
            if (!userExists)
            {
                return Result<bool>.Failure("NotFound", "User not found");
            }

            // Retrieve permissions
            var permissions = await _context.Permissions
                .Where(p => permissionDTOs.permissionIds.Contains(p.Id)).ToListAsync();

            if (permissions.Count != permissionDTOs.permissionIds.Count())
            {
                return Result<bool>.Failure("NotFound", "One or more permissions do not exist");
            }

            // Retrieve controller actions
            var controllerActions = await _context.ControllerActions
                .Where(ca => permissionDTOs.controllerActionIds.Contains(ca.Id)).ToListAsync();

            if (controllerActions.Count != permissionDTOs.controllerActionIds.Count())
            {
                return Result<bool>.Failure("NotFound", "One or more controller actions do not exist");
            }

            // Remove PermissionControllerAction entries
            var permissionControllerActions = _context.PermissionControllerActions
                .Where(pca => permissions.Select(p => p.Id).Contains(pca.PermissionId) &&
                              controllerActions.Select(ca => ca.Id).Contains(pca.ControlleractionId)).ToList();

            _context.PermissionControllerActions.RemoveRange(permissionControllerActions);

            // Remove permissions from the user
            var userPermissions = _context.UserPermissions
                .Where(up => up.UserId == permissionDTOs.userId &&
                             permissionDTOs.permissionIds.Contains(up.PermissionId)).ToList();

            _context.UserPermissions.RemoveRange(userPermissions);

            await _context.SaveChangesAsync();

            return Result<bool>.Success(true);
        }

        public Task<Result<AssignControllerActionToUserGetDTOs>> RemoveControllerActionToUserAsync(string userId, List<string> controlleractionId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<AssignPermissionGetDTOs>> RemovePermissionToUserAsync(string userId, List<string> permissionIds)
        {
            try
            {
                // Fetch user permissions based on userId and permissionIds
                var userPermissions = await _context.UserPermissions
                    .Where(up => up.UserId == userId && permissionIds.Contains(up.PermissionId))
                    .ToListAsync();

                if (!userPermissions.Any())
                {
                    return Result<AssignPermissionGetDTOs>.Failure("NotFound", "Permissions not found for the specified user.");
                }

                // Remove the permissions
                _context.UserPermissions.RemoveRange(userPermissions);
                await _context.SaveChangesAsync();

                var assignedPermissions = userPermissions.
                    Select(up => _mapper.Map<AssignPermissionGetDTOs>(up)).ToList();

                return Result<AssignPermissionGetDTOs>.Success(new AssignPermissionGetDTOs(userId, assignedPermissions));


            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while removig Permission");
            }
        }
    }
}
