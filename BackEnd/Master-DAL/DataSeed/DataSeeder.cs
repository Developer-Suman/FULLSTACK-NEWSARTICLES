
using Master_DAL.DbContext;
using Master_DAL.Interface;
using Master_DAL.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Transactions;

namespace Master_DAL.DataSeed
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Assembly _assembly;
        private readonly IActionDescriptorCollectionProvider _actionDescriptorCollectionProvider;

        public DataSeeder(IActionDescriptorCollectionProvider actionDescriptorCollectionProvider,ApplicationDbContext applicationDbContext, IUnitOfWork unitOfWork )
        {
            _context = applicationDbContext;
            _unitOfWork = unitOfWork;
            _actionDescriptorCollectionProvider = actionDescriptorCollectionProvider;
            _assembly = Assembly.GetExecutingAssembly();
        }

        public async Task Seed()
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    await SeedControllerAction();
                    await PermissionSeeding();
                    scope.Complete();
                }
                catch (Exception ex) {
                    scope.Dispose();
                    throw;
                }
            }
        }

        private async Task SeedControllerAction()
        {
            if (! await _context.PermissionControllerActions.AnyAsync())
            {
                var controllerActionInfos = new List<ControllerAction>();
                // Get all action descriptors
                var actionDescriptors = _actionDescriptorCollectionProvider.ActionDescriptors.Items;

                // Extract controller names
                var controllerNames = actionDescriptors
                    .OfType<ControllerActionDescriptor>()
                    .Select(ad => ad.ControllerName)
                    .Distinct()
                    .ToList();

                foreach (var desciptor in actionDescriptors.OfType<ControllerActionDescriptor>())
                {
                    var controllerName = desciptor.ControllerName;
                    var actionName = desciptor.ActionName;


                    var controllerAction = new ControllerAction(
                       id: Guid.NewGuid().ToString(),
                       controller: controllerName,
                       action: actionName
                    );

                    controllerActionInfos.Add(controllerAction);
                }


                // Save the list to your database
                _context.ControllerActions.AddRange(controllerActionInfos);
                await _context.SaveChangesAsync();
            }
        }

        private async Task PermissionSeeding()
        {
            if(!await _context.Permissions.AnyAsync())
            {
                var permissions = new List<Permission>()
                {
                    new Permission("1","Read"),
                    new Permission("2", "Update"),
                    new Permission("3","Delete"),
                    new Permission("4","Create")
                };
                await _context.Permissions.AddRangeAsync(permissions);  
                await _context.SaveChangesAsync();

            }

        }
    }
}
