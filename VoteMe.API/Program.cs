using VoteMe.API.Extension;
using VoteMe.Infrastructure.Extension;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();
builder.Services.AddVoteMeInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseApiMiddleware();

app.Run();

