
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SynthShop.Domain.Entities;

namespace SynthShop.Infrastructure.Data.Seed
{
    public class UserSeeder : ISeeder
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly Serilog.ILogger _logger;

        public UserSeeder(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager, Serilog.ILogger logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task SeedAsync()
        {
            await SeedRolesAsync();

            await SeedAdminUserAsync();
        }

        private async Task SeedRolesAsync()
        {
            var roles = new[]
            {
                new IdentityRole<Guid> { Id = Guid.Parse("F97157E2-7770-48DF-ADE9-C2E695DFEDD1"), Name = "User", NormalizedName = "USER" },
                new IdentityRole<Guid> { Id = Guid.Parse("B3553750-E886-4173-8E81-D70F0C3604AA"), Name = "Admin", NormalizedName = "ADMIN" }
            };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role.Name))
                {
                    var roleResult = await _roleManager.CreateAsync(role);
                    if (!roleResult.Succeeded)
                    {
                        _logger.Warning("Issue with seed roles");
                    }
                }
            }
        }

        private async Task SeedAdminUserAsync()
        {
            const string adminEmail = "admin@example.com";
            const string adminPassword = "Admin@123456";
            const string adminRoleName = "Admin";

            if (await _userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "User",
                    Address = "Admin Address",
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdateAt = null
                };

                var createUserResult = await _userManager.CreateAsync(adminUser, adminPassword);
                if (!createUserResult.Succeeded)
                {
                    _logger.Warning("Issue with seeding admin user");
                    return;
                }

                var addToRoleResult = await _userManager.AddToRoleAsync(adminUser, adminRoleName);
                if (!addToRoleResult.Succeeded)
                {
                    _logger.Warning("Issue with adding role to admin user");
                }
            }
        }
    }
}
