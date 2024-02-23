using AutoMapper;
using Master_BLL.Repository.Implementation;
using Master_BLL.Repository.Interface;
using Master_BLL.Services.Implementation;
using Master_BLL.Services.Interface;
using Master_DAL.Abstraction;
using Master_DAL.JWT;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.OpenApi.Models;

namespace MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Configs
{
    public static class DipendencyInjection
    {
        public static void Inject(WebApplicationBuilder builder)
        {

            #region Configuration
            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(
                option =>
                {
                    option.SwaggerDoc("v1", new OpenApiInfo { Title = "MASTER PROJECT API", Version = "V1" });
                    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter a valid token",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "Bearer"
                    });
                    option.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type=ReferenceType.SecurityScheme,
                                    Id="Bearer"
                                }
                            },
                            Array.Empty<string>()
                        }
                    });
                }
            );
            #endregion
            #region InjectDependency
            builder.Services.AddAuthorization();
            builder.Services.AddAuthorization();
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IJwtProvider, JwtProvider>();
            builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();
            builder.Services.AddScoped<IAccountServices, AccountServices>();
            builder.Services.AddMemoryCache();
            builder.Services.AddScoped<IMemoryCacheRepository, MemoryCacheRepository>();
            builder.Services.AddScoped<IUploadImageRepository, UploadImageRepository>();
            builder.Services.AddScoped<IArticlesRepository, ArticlesRepository>();

       
            #endregion

        }
    }
}
