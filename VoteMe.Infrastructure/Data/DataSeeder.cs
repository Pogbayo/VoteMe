using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using VoteMe.Domain.Constants;
using VoteMe.Domain.Entities;

namespace VoteMe.Infrastructure.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            await SeedRolesAsync(services);
            //await SeedSuperAdminAsync(services);
        }

        private static async Task SeedRolesAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

            foreach (var role in new[] { Roles.SuperAdmin, Roles.OrgAdmin, Roles.Voter })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new AppRole { Name = role });
            }
        }

        //private static async Task SeedSuperAdminAsync(IServiceProvider services)
        //{
        //    var userManager = services.GetRequiredService<UserManager<AppUser>>();
        //    var settings = services.GetRequiredService<IOptions<SuperAdminSettings>>().Value;

        //    // Note: SuperAdmin is seeded separately and does not require an organization
        //    var superAdmin = await userManager.FindByEmailAsync(settings.Email);
        //    if (superAdmin == null)
        //    {
        //        superAdmin = new AppUser
        //        {
        //            FirstName = settings.FirstName,
        //            LastName = settings.LastName,
        //            Email = settings.Email,
        //            UserName = settings.Email,
        //            DisplayName = settings.DisplayName,
        //        };

        //        await userManager.CreateAsync(superAdmin, settings.Password);
        //        await userManager.AddToRoleAsync(superAdmin, Roles.SuperAdmin);
        //    }
        //}
    }
}