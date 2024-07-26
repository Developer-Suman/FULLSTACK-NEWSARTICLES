using AutoMapper;
using Master_BLL.DTOs.Articles;
using Master_BLL.DTOs.Authentication;
using Master_BLL.DTOs.Permission;
using Master_BLL.DTOs.Permission.PermissionController;
using Master_BLL.DTOs.Permission.PermissionUser;
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
                // Check if the user exists
                var userExists = await _context.ApplicationUsers.AnyAsync(u => u.Id == permissionDTOs.userId);
                if (!userExists)
                {
                    return Result<PermissionDTOs>.Failure("NotFound","User not Found");
                    
                }
                // Retrieve the specified permissions
                var permissions = await _context.Permissions
                    .Where(p=> permissionDTOs.permissionIds.Contains(p.Id)).ToListAsync();

                // Ensure all specified permissions exist
                if (permissions.Count != permissionDTOs.permissionIds.Count())
                {
                    return Result<PermissionDTOs>.Failure("NotFound", "Permissions Don not exists");
                }


                // Retrieve the specified controller actions
                var controllerActions = await _context.ControllerActions
                    .Where(ca => permissionDTOs.controllerActionIds.Contains(ca.Id)).ToListAsync();


                // Ensure all specified controller actions exist
                if (controllerActions.Count != permissionDTOs.controllerActionIds.Count())
                {
                    return Result<PermissionDTOs>.Failure("NotFound", "One or more controller actions do not exist.");
                }


                // Check for existing PermissionControllerAction entries for the user
                var existingPermissionControllerActions = await _context.PermissionControllerActions
                    .Where(pca => permissionDTOs.permissionIds.Contains(pca.PermissionId) &&
                                  permissionDTOs.controllerActionIds.Contains(pca.ControlleractionId))
                    .ToListAsync();

                // Check for existing UserPermission entries for the user
                var existingUserPermissions = await _context.UserPermissions
                    .Where(up => up.UserId == permissionDTOs.userId &&
                                 permissionDTOs.permissionIds.Contains(up.PermissionId))
                    .ToListAsync();


                //Check the user that exist in ControllerActions 
                if (existingUserPermissions.Any())
                {
                    var existingUserPermissionIds = existingUserPermissions.Select(up => up.PermissionId).ToList();
                    var existingUserPermissionControllerActions = existingPermissionControllerActions
                        .Where(pca => existingUserPermissionIds.Contains(pca.PermissionId))
                        .ToList();

                    //if (existingUserPermissionControllerActions.Any())
                    //{
                    //    return Result<PermissionDTOs>.Failure("Duplicate", "One or more PermissionControllerActions already exist for this user.");
                    //}
                }

                // Create PermissionControllerAction entries for new assignments || add only if ea.permissionId == pca.PermissionId && ea.ControllerActionId ==pca.ControllerActionId
                var newPermissionControllerActions = permissions
                    .SelectMany(p => controllerActions, (p, ca) => new PermissionControllerAction
                    {
                        PermissionId = p.Id,
                        ControlleractionId = ca.Id
                    })
                    .Where(pca => !existingPermissionControllerActions
                        .Any(ea => ea.PermissionId == pca.PermissionId && ea.ControlleractionId == pca.ControlleractionId))
                    .ToList();



                // Fetch existing PermissionControllerAction entries
                var userPermission = await _context.UserPermissions
                    .Where(up=>up.UserId ==permissionDTOs.userId).ToListAsync();

                // Fetch existing PermissionControllerAction entries associated with the user permissions
                var existingPermissionControllerActionsWithUsers = await _context.PermissionControllerActions
                    .Where(pca => userPermission.Select(up => up.PermissionId).Contains(pca.PermissionId))
                    .ToListAsync();

                // Determine PermissionControllerAction entries to remove
                var toRemovePermissionControllerActions = existingPermissionControllerActionsWithUsers
                    .Where(ea => !permissionDTOs.controllerActionIds.Contains(ea.ControlleractionId) ||
                                 !permissionDTOs.permissionIds.Contains(ea.PermissionId))
                    .ToList();


                // Add the new entries to the database
                if (newPermissionControllerActions.Any())
                {
                    _context.PermissionControllerActions.AddRange(newPermissionControllerActions);
                }


                // Remove the old entries from the database
                if (toRemovePermissionControllerActions.Any())
                {
                    _context.PermissionControllerActions.RemoveRange(toRemovePermissionControllerActions);
                }

                // Assign permissions to the user if not already assigned
                var newUserPermissions = permissions
                    .Where(p => !existingUserPermissions.Any(up => up.PermissionId == p.Id))
                    .Select(p => new UserPermission
                    {
                        UserId = permissionDTOs.userId,
                        PermissionId = p.Id
                    }).ToList();


                if (newUserPermissions.Any())
                {
                    _context.UserPermissions.AddRange(newUserPermissions);
                }



                // Fetch existing UserPermissio entries associated with the user permissions
                var existingUserPermissioWithUsers = await _context.UserPermissions
                    .Where(pca => userPermission.Select(up => up.PermissionId).Contains(pca.PermissionId))
                    .ToListAsync();

                // Determine UserPermissio entries to remove
                var toRemoveUserPermission = existingUserPermissioWithUsers
                    .Where(ea => !permissionDTOs.userId.Contains(ea.UserId) ||
                                 !permissionDTOs.permissionIds.Contains(ea.PermissionId))
                    .ToList();



                if (toRemoveUserPermission.Any())
                {
                    _context.UserPermissions.RemoveRange(toRemoveUserPermission);
                }

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

    

        public async Task<Result<AssignPermissionGetDTOs>> AssignPermissionToUserAsync(PermissionUserDTOs permissionUserDTOs)
        {
            try
            {
                var permissions = await _context.Permissions
                    .Where(x=> permissionUserDTOs.permissionIds.Contains(x.Id)).ToListAsync();

                if(permissions is  null)
                {
                    return Result<AssignPermissionGetDTOs>.Failure("NotFound", "Permission are not found");
                }

                var UserPermission = permissions
                   .Select(p => new UserPermission
                   {
                       UserId = permissionUserDTOs.userId,
                       PermissionId = p.Id
                   }).ToList();

                _context.UserPermissions.AddRange(UserPermission);
                await _context.SaveChangesAsync();

              
                var assignedPermissions = UserPermission.Select(up => _mapper.Map<AssignPermissionGetDTOs>(up)).ToList();

                return Result<AssignPermissionGetDTOs>.Success(new AssignPermissionGetDTOs(permissionUserDTOs.userId, assignedPermissions));
               


            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while AssigningPermission");
            }
        }

        public async Task<Result<List<UserDTOs>>> GetAllUser()
        {
            try
            {
                var getAllUser = await _context.Users.ToListAsync();
                var username = getAllUser.Select(x=> x.UserName).ToList();
                var result = _mapper.Map<List<UserDTOs>>(getAllUser);
                
                return Result<List<UserDTOs>>.Success(result);

            }
            catch (Exception ex) {
                throw new Exception("An error occured while Fetching User");
            }
        }

        public async Task<Result<ControllerActionUserGetDTOs>> GetControllerActionByUserId(string UserId)
        {
            try
            {
                var userPermissions = await _context.UserPermissions
                    .Where(up => up.UserId == UserId)
                    .Select(up => up.PermissionId)
                    .ToListAsync();

                if (!userPermissions.Any())
                {
                    return Result<ControllerActionUserGetDTOs>.Failure("NotFound", "No permissions found for the user.");
                }

                var controllerActionByPermission = await _context.PermissionControllerActions
                    .Where(pca => userPermissions.Contains(pca.PermissionId))
                    .Select(pca => pca.ControllerAction) 
                    .ToListAsync();

                var permissionContollersGetDTO = controllerActionByPermission
                    .Select(ca => new PermissionControllerGetDTOs(
                        controllerActionId :ca.Id,
                        controllerActionName: ca.Controller + ca.Action
                        )).ToList();



                var result = new ControllerActionUserGetDTOs(
                    UserId,
                    permissionContollersGetDTO
                    );

                return Result<ControllerActionUserGetDTOs>.Success(result);



            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while Fetching Data");
            }
        }

        public Task<Result<List<PermissionAuthorizedDTOs>>> GetPermissionAuthorizedData(string CurrentUser)
        {
            throw new NotImplementedException();
        }

        public async Task<Result<List<PermissionUserGetDTOs>>> GetPermissionByUserId(string UserId)
        {
            try
            {
                var getPermission = await _context.UserPermissions
                    .Where(x=> x.UserId == UserId)
                    .Select(x=>x.PermissionId)
                    .ToListAsync();

                if(!getPermission.Any())
                {
                    return Result<List<PermissionUserGetDTOs>>.Failure("NotFound", "No permissions found for the user.");
                }

                //Fetch Permission
                var permissions = await _context.Permissions
                    .Where(p=> getPermission.Contains(p.Id)).ToListAsync();


                var result = permissions
                    .Select(p => new PermissionUserGetDTOs
                    (
                        userId: UserId,
                        permissionId : p.Id,
                        permissionName : p.Permissions
                   )).ToList();

                return Result<List<PermissionUserGetDTOs>>.Success(result);
               
               
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while Getting Permission");
            }
        }

        public async Task<Result<AssignPermissionGetDTOs>> RemovePermissionToUserAsync(PermissionUserDTOs permissionUserDTOs)
        {
            try
            {
                // Fetch user permissions based on userId and permissionIds
                var userPermissions = await _context.UserPermissions
                    .Where(up => up.UserId == permissionUserDTOs.userId && permissionUserDTOs.permissionIds.Contains(up.PermissionId))
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

                return Result<AssignPermissionGetDTOs>.Success(new AssignPermissionGetDTOs(permissionUserDTOs.userId, assignedPermissions));


            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while removig Permission");
            }
        }
    }
}
