using VoteMe.API.Middleware;

namespace VoteMe.API.Extension
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            return services;
        }

        public static WebApplication UseApiMiddleware(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }
    }
}
