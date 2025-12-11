using WingDing_Party.AuthenticationService.Api;
using WingDing_Party.AuthenticationService.Application;
using WingDing_Party.AuthenticationService.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Services
        .AddPresentation()
        .AddApplication()
        .AddInfrastructure(builder.Configuration)
        ;
}

var app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/error-development");

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "WingDing Authentication API v1");
            c.RoutePrefix = string.Empty;
        });
    }
    else
    {
        app.UseExceptionHandler("/error");
    }

    app.UseHttpsRedirection();
    app.MapControllers();
    app.Run();

}

