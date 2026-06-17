using AuthService.Api;
using AuthService.Api.gRPC.Services;
using AuthService.Application;
using AuthService.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .AddPresentation(builder.Configuration)
        .AddInfrastructure(builder.Configuration)
        .AddApplication()
        ;

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(5200, o => o.Protocols = HttpProtocols.Http1);  // REST
        options.ListenAnyIP(5201, o => o.Protocols = HttpProtocols.Http2);  // gRPC (h2c)
    });
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
