using Master_BLL;
using Master_DAL;
using Master_DAL.Extensions;
using MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.OpenApi.Models;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;



try
{
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



    #region Logger Configuration
    //This logger facilitates only for debugger mode, When you want to use this for production mode, you can configure this in appsetting.js
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("logs/SumanLog-.txt", rollingInterval: RollingInterval.Minute)
        .CreateLogger();


    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration).Enrich.FromLogContext().CreateLogger();




    #endregion
    // Register Serilog with DI
    //Use both serialog and BuildIn Parallery
    builder.Services.AddLogging(loggingBuilder =>
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.AddSerilog(dispose: true);
    });
    //Alternatives


    builder.Host.UseSerilog((context, config) =>
    config.Enrich.FromLogContext()
          .ReadFrom.Configuration(context.Configuration));




    //builder.Services.AddResponseCompression(options =>
    //{
    //    options.EnableForHttps = true;
    //    options.Providers.Add<BrotliCompressionProvider>();
    //    //options.Providers.Add<GzipCompressionProvider>();
    //});

    //builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    //{
    //    options.Level = System.IO.Compression.CompressionLevel.SmallestSize;

    //});

    //builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    //{
    //    options.Level = System.IO.Compression.CompressionLevel.SmallestSize;
    //});


    var app = builder.Build();
   
    app.ConfigureCustomExceptionMiddleware();
    ApplicationConfiguration.Configure(app);


    //app.UseResponseCompression();
    app.Run();

}
catch(Exception ex)
{
    Log.Error("The following {Exception} was thrown during application startUp", ex);

}
finally
{
    Log.CloseAndFlushAsync();
}

