using Hangfire;
using VoteMe.API.Hubs;
using VoteMe.API.Middleware;
using VoteMe.API.Services;
using VoteMe.Application.Interface.IServices;

namespace VoteMe.API.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                }); services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });

            services.AddEndpointsApiExplorer();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddSwaggerGen();
            services.AddSignalR();
            return services;
        }

        public static WebApplication UseApiMiddleware(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            //app.UseMiddleware<OrgIdLoggingMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapControllers();
            app.MapHub<ElectionHub>("/hubs/election");

            app.UseHangfireDashboard("/hangfire");

            return app;
        }
    }
}
