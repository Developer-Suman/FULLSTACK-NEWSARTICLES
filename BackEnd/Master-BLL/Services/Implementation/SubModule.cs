using Master_BLL.DTOs.Menu;
using Master_BLL.DTOs.Modules;
using Master_BLL.DTOs.SubModules;
using Master_BLL.Services.Interface;
using Master_DAL.Abstraction;
using Master_DAL.DbContext;
using Master_DAL.Exceptions;
using Master_DAL.Interface;
using Master_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Master_BLL.Services.Implementation
{
    public class SubModule : ISubModule
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public SubModule(ApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork)
        {
            _context = applicationDbContext;
            _unitOfWork = unitOfWork;
            
        }
        public async Task<Result<SubModulesGetDTOs>> Add(SubModulesCreateDTOs subModulesCreateDTOs)
        {
            throw new NotImplementedException();
            //using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            //{
            //    try
            //    {
            //        string newId = Guid.NewGuid().ToString();
            //        var submodulesData = new SubModules(
            //        newId,
            //        subModulesCreateDTOs.name,
            //        subModulesCreateDTOs.iconUrl,
            //        subModulesCreateDTOs.targetUrl,
            //        subModulesCreateDTOs.role,
            //        subModulesCreateDTOs.moduleId,
            //        subModulesCreateDTOs.rank

            //        );

            //        await _unitOfWork.Repository<SubModules>().AddAsync(submodulesData);
            //        await _unitOfWork.SaveChangesAsync();
            //        scope.Complete();
            //        var resultDTOs = new SubModulesGetDTOs(
            //            submodulesData.Id,
            //            submodulesData.Name,
            //            submodulesData.iconUrl,
            //            submodulesData.TargetUrl,
            //            submodulesData.Role,
            //            submodulesData.Rank,
            //            submodulesData.Menu.Select(sm => new MenuGetDTOs(
            //                sm.Id,
            //                sm.Name,
            //                sm.IconUrl,
            //                sm.TargetUrl,
            //                sm.Role,
            //                sm.Rank
            //                )).ToList()
            //            );

            //        return Result<SubModulesGetDTOs>.Success(resultDTOs);

            //    }
            //    catch (Exception ex)
            //    {
            //        scope.Dispose();
            //        throw new ConflictException("An Exception occured while Adding SubModules");
            //    }

            //}
        }
    }
}
