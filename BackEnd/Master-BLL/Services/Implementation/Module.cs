using Master_BLL.DTOs.Menu;
using Master_BLL.DTOs.Modules;
using Master_BLL.DTOs.SubModules;
using Master_BLL.Services.Interface;
using Master_DAL.Abstraction;
using Master_DAL.DbContext;
using Master_DAL.Exceptions;
using Master_DAL.Interface;
using Master_DAL.Models;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace Master_BLL.Services.Implementation
{
    public class Module : IModule
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public Module(ApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork)
        {
            _context = applicationDbContext;
            _unitOfWork = unitOfWork;
            
        }
        public async Task<Result<ModulesGetDTOs>> Add(ModulesCreateDTOs modulesCreateDTOs)
        {
            throw new NotImplementedException();
            //using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //{
            //    try
            //    {
            //        string newId = Guid.NewGuid().ToString();
            //        var modulesData = new Modules(
            //        newId,
            //        modulesCreateDTOs.Name,
            //        modulesCreateDTOs.Role,
            //        modulesCreateDTOs.TargetUrl
            //        );

            //        await _unitOfWork.Repository<Modules>().AddAsync(modulesData);
            //        await _unitOfWork.SaveChangesAsync();
            //        scope.Complete();
            //        var resultDTOs = new ModulesGetDTOs(
            //            modulesData.Id,
            //            modulesData.Name,
            //            modulesData.Role,
            //            modulesData.TargetUrl,
            //            modulesData.SubModules.Select(sm => new SubModulesGetDTOs(
            //                sm.Id,
            //                sm.Name,
            //                sm.iconUrl,
            //                sm.TargetUrl,
            //                sm.Role,
            //                sm.Rank,
            //                sm.Menu.Select(m => new MenuGetDTOs(
            //                    m.Id,
            //                    m.Name,
            //                    m.IconUrl,
            //                    m.TargetUrl,
            //                    m.Role,
            //                    m.Rank
            //                    )
            //                ).ToList())
            //            ).ToList());

            //        return Result<ModulesGetDTOs>.Success(resultDTOs);

            //    }
            //    catch (Exception ex)
            //    {
            //        scope.Dispose();
            //        throw new ConflictException("An Exception occured while Adding Modules");
            //    }

            //}
        }

        public async Task<Result<GetModulesRoles>> AssignModulesToRole(string roleId, IEnumerable<string> moduleIds)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    var existingAssignment = await _context.RoleModules
                        .Where(rm => rm.Id == roleId)
                        .ToListAsync();

                    if (existingAssignment != null)
                    {
                        // Determine existing module IDs
                        var existingModuleIds = existingAssignment.Select(rm => rm.ModuleId).ToList();

                        // Identify modules to remove
                        var modulesToRemove = existingAssignment
                            .Where(rm => !moduleIds.Contains(rm.ModuleId))
                            .ToList();

                        if (modulesToRemove.Any())
                        {
                            _context.RoleModules.RemoveRange(modulesToRemove);
                        }
                    }

                    // Determine new modules to add
                    var newModuleIds = moduleIds.Except(existingAssignment.Select(rm => rm.ModuleId));

                    foreach (var moduleId in newModuleIds)
                    {
                        var roleModule = new RoleModule
                        {
                            RoleId = roleId,
                            ModuleId = moduleId
                        };
                        _context.RoleModules.Add(roleModule);
                    }

                    await _context.SaveChangesAsync();

                    scope.Complete();

                    var resultDTOs = new GetModulesRoles(
                        roleId,
                       newModuleIds.Select(id => id.ToString()).ToList()
                        );

                    return Result<GetModulesRoles>.Success(resultDTOs);

                }
                catch (Exception ex)
                {
                    scope.Dispose();
                    throw new Exception("An error occured while assign modules to Roles");
                }
            }
        }

        public async Task<Result<ModulesGetDTOs>> GetModuleWithDetails(string moduleId)
        {
            throw new NotImplementedException();
            //try
            //{
            //    // Ensure you await the async method
            //    var module = await _context.Modules
            //        .Include(x => x.SubModules)
            //            .ThenInclude(sm => sm.Menu) // Make sure this matches the property name
            //        .SingleOrDefaultAsync(x => x.Id == moduleId);

            //    if (module == null)
            //    {
            //        return Result<ModulesGetDTOs>.Failure("NotFound", "Module not found");
            //    }

            //    var resultDTOs = new ModulesGetDTOs(
            //        module.Id,
            //        module.Name,
            //        module.Role,
            //        module.TargetUrl,
            //        module.SubModules.Select(sm => new SubModulesGetDTOs(
            //            sm.Id,
            //            sm.Name,
            //            sm.iconUrl,
            //            sm.TargetUrl,
            //            sm.Role,
            //            sm.Rank,
            //            sm.Menu.Select(m => new MenuGetDTOs(
            //                m.Id,
            //                m.Name,
            //                m.IconUrl,
            //                m.TargetUrl,
            //                m.Role,
            //                m.Rank
            //            )).ToList()
            //        )).ToList()
            //    );

            //    return Result<ModulesGetDTOs>.Success(resultDTOs);
            //}
            //catch (Exception ex)
            //{
            //    // Optionally, log the exception
            //    throw new ApplicationException("An error occurred while fetching module details.", ex);
            //}
        }

        //public async Task<Result<ModulesGetDTOs>> GetNavigationMenuByUser(string userId)
        //{
        //    try
        //    {
        //        var roles = await _context.UserRoles
        //              .Where(ur => ur.UserId == userId)
        //              .Select(ur => ur.RoleId)
        //              .ToListAsync();


        //        var modules = await _context.RoleModules
        //             .Where(rm => roles.Contains(rm.RoleId))
        //             .Select(rm => rm.Modules)
        //             .Distinct()
        //             .Include(m => m.SubModules)
        //             .ThenInclude(sm => sm.Menu)
        //             .ToListAsync();




        //        var resultDTOs = new ModulesGetDTOs(
        //           modules.FirstOrDefault()?.Id ?? string.Empty, 
        //           modules.FirstOrDefault()?.Name ?? string.Empty,
        //           modules.FirstOrDefault()?.Role,
        //           modules.FirstOrDefault()?.TargetUrl,
        //           modules.SelectMany(m => m.SubModules).Select(sm => new SubModulesGetDTOs(
        //               sm.Id,
        //               sm.Name,
        //               sm.iconUrl,
        //               sm.TargetUrl,
        //               sm.Role,
        //               sm.Rank,
        //               sm.Menu.Select(menu => new MenuGetDTOs(
        //                   menu.Id,
        //                   menu.Name,
        //                   menu.IconUrl,
        //                   menu.TargetUrl,
        //                   menu.Role,
        //                   menu.Rank
        //               )).ToList()
        //           )).ToList()
        //        );

        //        return Result<ModulesGetDTOs>.Success( resultDTOs );

        //    }
        //    catch (Exception ex) 
        //    {
        //        throw new Exception("An error occured while Getting Navigation Menu");
        //    }
        //}


        public async Task<Result<List<ModulesGetDTOs>>> GetNavigationMenuByUser(string userId)
        {
            //throw new NotImplementedException();
            try
            {
                // Retrieve all roles for the user
                var roles = await _context.UserRoles
                    .Where(ur => ur.UserId == userId)
                    .Select(ur => ur.RoleId)
                    .ToListAsync();

                // Retrieve modules associated with these roles, including submodules and menus
                var modules = await _context.RoleModules
                    .Where(rm => roles.Contains(rm.RoleId))
                    .Select(rm => rm.Modules)
                    .Distinct()
                    .Include(m => m.SubModules)
                    .ThenInclude(sm => sm.Menu)
                    .ToListAsync();



                // Map the retrieved data to the DTO structure
                var resultDTO = modules.Select(m => new ModulesGetDTOs(
                    m.Id,
                    m.Name,
                    m.Role,
                    m.TargetUrl,
                    m.SubModules.Select(sm => new SubModulesGetDTOs(
                       sm.Id,
                       sm.Name,
                       sm.iconUrl,
                       sm.TargetUrl,
                       sm.Role,
                       sm.Rank
                    )).ToList(),
                     m.SubModules.SelectMany(sm => sm.Menu.Select(menu => new MenuGetDTOs(
                        menu.Id,
                           menu.Name,
                           menu.IconUrl,
                           menu.TargetUrl,
                           menu.Role,
                           menu.Rank
                    ))).ToList()))
                    .ToList();



                // Return the result wrapped in a success response
                return Result<List<ModulesGetDTOs>>.Success(resultDTO);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while getting the navigation menu.", ex);
            }
        }



        public async Task<Result<GetModulesRoles>> RemoveModulesFromRole(string roleId, IEnumerable<string> moduleIds)
        {
            try
            {
                var rolesModulesRemoves = await _context.RoleModules
                    .Where(rm=>rm.RoleId == roleId && moduleIds.Contains(rm.ModuleId))
                    .ToListAsync();

                if (rolesModulesRemoves.Any())
                {
                    _context.RoleModules.RemoveRange(rolesModulesRemoves);
                    await _context.SaveChangesAsync();
                }

                var resultDTOs = new GetModulesRoles
                    (
                    roleId,
                    rolesModulesRemoves.Select(id => id.ToString()).ToList()
                    );

                return Result<GetModulesRoles>.Success(resultDTOs);



            }
            catch (Exception ex) 
            {
                throw new Exception("An error occured while Removing Roles");
            }
        }

        public Task<Result<GetModulesRoles>> RemoveModulesFromRoleAsync(string roleId, IEnumerable<int> moduleIds)
        {
            throw new NotImplementedException();
        }
    }
}
