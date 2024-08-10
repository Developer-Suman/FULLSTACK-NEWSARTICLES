using Master_BLL.DTOs.Menu;
using Master_BLL.DTOs.SubModules;
using Master_BLL.Services.Interface;
using Master_DAL.Abstraction;
using Master_DAL.DbContext;
using Master_DAL.Exceptions;
using Master_DAL.Interface;
using Master_DAL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Master_BLL.Services.Implementation
{
    public class MenuServices : IMenu
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        public MenuServices(IUnitOfWork unitOfWork, ApplicationDbContext applicationDbContext, RoleManager<IdentityRole> roleManager )
        {
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _context = applicationDbContext;
            
        }
        public async Task<Result<MenuGetDTOs>> Add(MenuCreateDTOs menuCreateDTOs)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) 
            {
                try
                {
                    string newId = Guid.NewGuid().ToString();

                    if (menuCreateDTOs.role is not null && !(await _roleManager.RoleExistsAsync(menuCreateDTOs.role)))
                    {
                        return Result<MenuGetDTOs>.Failure("Role doesnot Exists");
                    }
                    var menuData = new Menu(
                    newId,
                    menuCreateDTOs.name,
                    menuCreateDTOs.targetUrl,
                    menuCreateDTOs.icon,
                    menuCreateDTOs.role,
                    menuCreateDTOs.moduleId,
                    menuCreateDTOs.rank
                    
                    );

                    await _unitOfWork.Repository<Menu>().AddAsync(menuData);
                    await _unitOfWork.SaveChangesAsync();
                    scope.Complete();
                    var resultDTOs = new MenuGetDTOs(
                        menuData.Id,
                        menuData.Name,
                        menuData.IconUrl,
                        menuData.TargetUrl,
                        menuData.Role,
                        menuData.Rank
                        );

                    return Result<MenuGetDTOs>.Success(resultDTOs);

                }
                catch (Exception ex) 
                {
                    scope.Dispose();
                    throw new ConflictException("An Exception occured while Adding SubModules");
                }

            }
        }
    }
}
