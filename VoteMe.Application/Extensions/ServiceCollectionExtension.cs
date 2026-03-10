using Microsoft.Extensions.DependencyInjection;
using VoteMe.Application.Interface.IServices;
using VoteMe.Application.Services;

namespace VoteMe.Application.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<IOrganizationMemberService, OrganizationMemberService>();
            services.AddScoped<IElectionService, ElectionService>();
            services.AddScoped<IElectionCategoryService, ElectionCategoryService>();
            services.AddScoped<ICandidateService, CandidateService>();
            services.AddScoped<IVoteService, VoteService>();

            return services;
        }
    }
}