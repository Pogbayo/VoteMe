using VoteMe.API.Extension;
using VoteMe.Infrastructure.Extension;
using VoteMe.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();
builder.Services.AddVoteMeInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseApiMiddleware();

app.Run();

