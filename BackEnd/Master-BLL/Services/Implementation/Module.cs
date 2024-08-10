using Master_BLL.DTOs.Articles;
using Master_BLL.DTOs.Menu;
using Master_BLL.DTOs.Modules;
using Master_BLL.DTOs.SubModules;
using Master_BLL.Repository.Implementation;
using Master_BLL.Services.Interface;
using Master_DAL.Abstraction;
using Master_DAL.DbContext;
using Master_DAL.Exceptions;
using Master_DAL.Interface;
using Master_DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) 
            {
                try
                {
                    string newId = Guid.NewGuid().ToString();
                    var modulesData = new Modules(
                    newId,
                    modulesCreateDTOs.Name,
                    modulesCreateDTOs.Role,
                    modulesCreateDTOs.TargetUrl
                    );

                    await _unitOfWork.Repository<Modules>().AddAsync(modulesData);
                    await _unitOfWork.SaveChangesAsync();
                    scope.Complete();
                    var resultDTOs = new ModulesGetDTOs(
                        modulesData.Id,
                        modulesData.Name,
                        modulesData.Role,
                        modulesData.TargetUrl,  
                        modulesData.SubModules.Select(sm=> new SubModulesGetDTOs(
                            sm.Id,
                            sm.Name,
                            sm.iconUrl,
                            sm.TargetUrl,
                            sm.Role,
                            sm.Rank,
                            sm.Menu.Select(m=>new MenuGetDTOs(
                                m.Id,
                                m.Name,
                                m.IconUrl,
                                m.TargetUrl,
                                m.Role,
                                m.Rank
                                )
                            ).ToList())
                        ).ToList());

                    return Result<ModulesGetDTOs>.Success(resultDTOs);

                } catch (Exception ex) {
                    scope.Dispose();
                    throw new ConflictException("An Exception occured while Adding Modules");
                }

            }
        }
        public async Task<Result<ModulesGetDTOs>> GetModuleWithDetailsAsync(string moduleId)
        {
            try
            {
                // Ensure you await the async method
                var module = await _context.Modules
                    .Include(x => x.SubModules)
                        .ThenInclude(sm => sm.Menu) // Make sure this matches the property name
                    .SingleOrDefaultAsync(x => x.Id == moduleId);

                if (module == null)
                {
                    return Result<ModulesGetDTOs>.Failure("NotFound", "Module not found");
                }

                var resultDTOs = new ModulesGetDTOs(
                    module.Id,
                    module.Name,
                    module.Role,
                    module.TargetUrl,
                    module.SubModules.Select(sm => new SubModulesGetDTOs(
                        sm.Id,
                        sm.Name,
                        sm.iconUrl,
                        sm.TargetUrl,
                        sm.Role,
                        sm.Rank,
                        sm.Menu.Select(m => new MenuGetDTOs(
                            m.Id,
                            m.Name,
                            m.IconUrl,
                            m.TargetUrl,
                            m.Role,
                            m.Rank
                        )).ToList()
                    )).ToList()
                );

                return Result<ModulesGetDTOs>.Success(resultDTOs);
            }
            catch (Exception ex)
            {
                // Optionally, log the exception
                throw new ApplicationException("An error occurred while fetching module details.", ex);
            }
        }



    }
}
