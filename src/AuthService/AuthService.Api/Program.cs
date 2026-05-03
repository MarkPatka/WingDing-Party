using AuthService.Api;
using AuthService.Infrastructure;
using AuthService.Application;

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

    app.ApplyMigrations();
    app.UseHttpsRedirection();
    app.UseExceptionHandler();
    //app.MapControllers();
    app.Run();
}
