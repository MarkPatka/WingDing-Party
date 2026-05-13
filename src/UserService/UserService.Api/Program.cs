using UserService.Api;
using UserService.Api.Mapping;
using UserService.Application;
using UserService.Application.Common.Mapping;
using UserService.Infrastructure;
using UserService.Infrastructure.Messaging.Mapping;


var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .AddPresentation(builder.Configuration)
        .AddMappings(
            typeof(UserProfileMappingConfiguration).Assembly,
            typeof(UserIntegrationMappingConfiguration).Assembly)
        .AddApplication()
        .AddInfrastructure(builder.Configuration)
        .AddControllers();
}


var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    
    app.ApplyMigrations();
    
    app.UseHttpsRedirection();
    app.UseExceptionHandler();
    app.MapControllers();
    app.Run();
}