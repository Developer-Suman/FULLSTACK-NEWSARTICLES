using AutoMapper;
using Master_BLL.Repository.Implementation;
using Master_BLL.Repository.Interface;
using Master_BLL.Services.Implementation;
using Master_BLL.Services.Interface;
using Master_DAL.Abstraction;
using Master_DAL.JWT;
using MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Configs;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_BLL
{
    public static class AssemblyReference
    {
        public static IServiceCollection AddBLL(this IServiceCollection services)
        {

            //CORS Enable
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
            });

            // Auto Mapper Configurations
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);


            #region InjectDependency
            services.AddAuthorization();
            //builder.Services.AddAuthorization();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            services.AddScoped<IAccountServices, AccountServices>();
            services.AddMemoryCache();
            services.AddScoped<IMemoryCacheRepository, MemoryCacheRepository>();
            services.AddScoped<IUploadImageRepository, UploadImageRepository>();
            services.AddScoped<IArticlesRepository, ArticlesRepository>();
            services.AddScoped<IHelpherMethods, HelpherMethods>();
            services.AddScoped<ICommentsRepository, CommentsRepository>();
            services.AddScoped<IPermissionServices, PermissionServices>();

            //builder.Services.Add(new ServiceDescriptor(
            //    typeof(IArticlesRepository),
            //    typeof(ArticlesRepository),
            //    ServiceLifetime.Transient
            //    ));


            #endregion

            return services;
        }
    }
}
