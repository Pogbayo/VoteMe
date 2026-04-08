using VoteMe.API.Extension;
using VoteMe.Infrastructure.Extension;
using VoteMe.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile("appsettings.Local.json", optional: true);

builder.Services.AddApiServices();
builder.Services.AddVoteMeInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();
 
//using (var scope = app.Services.CreateScope())
//{
//    await DataSeeder.SeedAsync(scope.ServiceProvider);
//}

app.UseApiMiddleware();

app.Run();

