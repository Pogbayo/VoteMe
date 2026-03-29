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
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddSwaggerGen();
            services.AddSignalR();
            return services;
        }

        public static WebApplication UseApiMiddleware(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

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
