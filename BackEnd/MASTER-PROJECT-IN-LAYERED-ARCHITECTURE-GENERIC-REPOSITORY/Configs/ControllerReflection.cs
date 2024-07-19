using Master_DAL.DbContext;
using Master_DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using System.Reflection;

namespace MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Configs
{
    public static class ControllerReflection
    {
        public static void InitializePermissionTable(ApplicationDbContext dbContext)
        {
            var controllers = Assembly.GetExecutingAssembly().GetTypes()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type));

            foreach (var controller in controllers)
            {
                var methods = controller.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
                foreach (var method in methods)
                {
                    Permission permission = new()
                    {
                        ActionName = method.Name,
                        ControllerName = controller.Name.Replace("Controller", ""),
                    };
                    dbContext.Permissions.Add(permission);

                }
            }
            dbContext.SaveChanges();
           
        }

      

    }
}
