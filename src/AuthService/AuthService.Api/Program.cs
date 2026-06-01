using AuthService.Api;
using AuthService.Api.gRPC.Services;
using AuthService.Application;
using AuthService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .AddPresentation(builder.Configuration)
        .AddInfrastructure(builder.Configuration)
        .AddApplication()
        ;
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    
    app.UseAuthentication();  // JWT validation middleware
    app.UseAuthorization();   // Permission checking middleware 
    
    app.ApplyMigrations();
    app.UseHttpsRedirection();
    app.UseExceptionHandler();
    app.MapControllers();
    app.MapGrpcService<PermissionGrpcService>(); // TODO: create and move to app extension
    app.Run();
}
