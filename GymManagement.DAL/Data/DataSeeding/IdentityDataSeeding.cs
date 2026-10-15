using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GymManagement.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GymManagement.DAL.Data.DataSeeding
{
    public static class IdentityDataSeeding
    {
        public static async Task SeedIdentityDataAsync(RoleManager<IdentityRole> roleManager ,
            UserManager<ApplicationUser> userManager ,
            ILogger logger,
            CancellationToken ct = default)
        {
            try
            {
                var hasUsers = await userManager.Users.AnyAsync();
                var hasRoles = await roleManager.Roles.AnyAsync();
                if (hasUsers && hasRoles) return;

                var roles = new List<IdentityRole>()
                {
                new IdentityRole("SuperAdmin"),
                new IdentityRole("Admin")
                };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role.Name!))
                    {
                        var roleResult = await roleManager.CreateAsync(role);
                        if (!roleResult.Succeeded)
                        {
                            logger.LogError($"Failed to Create Role {role.Name} : {string.Join(" ; ", roleResult.Errors.Select(e => e.Description))}");
                        }
                    }
                }

                if (!hasUsers)
                {
                    var MainAdmin = new ApplicationUser()
                    {
                        FirstName = "Mohamed",
                        LastName = "Hassan",
                        Email = "mohamedhassan1732007@gmail.com",
                        UserName = "MohamedHassan",
                        PhoneNumber = "01032438307"
                    };

                    await userManager.CreateAsync(MainAdmin, "P@ssw0rd");
                    await userManager.AddToRoleAsync(MainAdmin, "SuperAdmin");

                    var Admin = new ApplicationUser()
                    {
                        FirstName = "Abo",
                        LastName = "Hassan",
                        Email = "mohamedhassan17307@gmail.com",
                        UserName = "AboHassan",
                        PhoneNumber = "01119007136"
                    };

                    await userManager.CreateAsync(Admin, "P@ssw0rd");
                    await userManager.AddToRoleAsync(Admin, "Admin");

                    logger.LogInformation($"Identity Data Seeded");
                }
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Identity Seeding Failed");
                return;
            }

        }
    }
}
