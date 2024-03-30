using Master_BLL;
using Master_DAL;
using Master_DAL.Extensions;
using MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);




ConfigurationManager configuration = builder.Configuration;
builder.Services
    .AddBLL()
    .AddDAL(configuration);
    
    

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
var app = builder.Build();

app.ConfigureCustomExceptionMiddleware();
ApplicationConfiguration.Configure(app);



app.Run();
