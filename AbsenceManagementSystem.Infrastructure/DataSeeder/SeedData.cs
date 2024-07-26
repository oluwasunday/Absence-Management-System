using AbsenceManagementSystem.Core.Domain;
using AbsenceManagementSystem.Core.Enums;
using AbsenceManagementSystem.Infrastructure.DbContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AbsenceManagementSystem.Infrastructure.DataSeeder
{
    public static class SeedData
    {
        public static async Task Seed(IServiceProvider serviceProvider)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AMSDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Employee>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();


                await context.Database.MigrateAsync();

                // Look for any data, if there is data already, then do nothing
                if (context.Users.Any())
                {
                    return;   // DB has been seeded
                }



                // Add seed data
                if (!context.Roles.Any())
                {
                    var roles = new[] { "Admin", "Employee" };
                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }
                    }
                }
                    

                var users = new List<Employee> {
                    new Employee
                    {
                        Id = "b55acd4a-57ff-4fc7-8643-f57bd9942df1",
                        FirstName = "Roseline",
                        LastName = "Admin",
                        UserName = "roseline",
                        Email = "admin@gmail.com",
                        PhoneNumber = "09043546576",
                        Gender = Gender.Female.ToString(),
                        DateOfBirth = new DateTime(2000, 2, 21),
                        MaritalStatus = "Married",
                        IsActive = true,
                        DateCreated= DateTime.Now,
                        DateModified = DateTime.Now,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true

                    },
                    new Employee
                    {
                        Id = "a2e492d5-ca29-4b39-ada6-d833d7aaba27",
                        FirstName = "Tester",
                        LastName = "Test",
                        UserName = "tester",
                        Email = "tester@gmail.com",
                        PhoneNumber = "09043546588",
                        Gender = Gender.Male.ToString(),
                        DateOfBirth = new DateTime(2000, 2, 21),
                        MaritalStatus = "Single",
                        IsActive = true,
                        DateCreated= DateTime.Now,
                        DateModified = DateTime.Now,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true
                    }
                };

                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "Passwd@123!");
                    if (user == users[0])
                        await userManager.AddToRoleAsync(user, "Admin");
                    else
                        await userManager.AddToRoleAsync(user, "Employee");
                }
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }   
    }
}
    

