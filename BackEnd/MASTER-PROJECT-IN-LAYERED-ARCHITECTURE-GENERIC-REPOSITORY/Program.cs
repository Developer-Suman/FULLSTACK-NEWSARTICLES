using Master_BLL;
using Master_DAL;
using MASTER_PROJECT_IN_LAYERED_ARCHITECTURE_GENERIC_REPOSITORY.Configs;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);


DipendencyInjection.Inject(builder);
ConfigurationManager configuration = builder.Configuration;
builder.Services
    .AddBLL()
    .AddDAL(configuration);

var app = builder.Build();



ApplicationConfiguration.Configure(app);

app.Run();
