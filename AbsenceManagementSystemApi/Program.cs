using AbsenceManagementSystem.Core.Domain;
using AbsenceManagementSystem.Core.DTO;
using AbsenceManagementSystem.Core.IRepositories;
using AbsenceManagementSystem.Core.IServices;
using AbsenceManagementSystem.Core.UnitOfWork;
using AbsenceManagementSystem.Infrastructure.DataSeeder;
using AbsenceManagementSystem.Infrastructure.DbContext;
using AbsenceManagementSystem.Infrastructure.Repositories;
using AbsenceManagementSystem.Services.Services;
using AbsenceManagementSystemApi.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace AbsenceManagementSystemApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            /*var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables() // Add environment variables
                .Build();*/

            // Add services to the container.


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

            // For Entity Framework Core
            builder.Services.AddDbContext<AMSDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AbsenceMgtConn")));

            var apiKey2 = builder.Configuration["EMAIL_API_KEY"];
            var apiKey = Environment.GetEnvironmentVariable("EMAIL_API_KEY");
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("Email API key is not set.");
            }

            var conFig = builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddEnvironmentVariables().Build();
            builder.Services.AddSingleton<IConfiguration>(conFig);

            builder.Services.AddIdentity<Employee, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                //options.Password.RequiredUniqueChars = 1;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = false;
            })
            .AddEntityFrameworkStores<AMSDbContext>()
            .AddDefaultTokenProviders();

            //
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IEmployeeLeaveRequestService, EmployeeLeaveRequestService>();
            builder.Services.AddScoped<ILeaveTypeService, LeaveTypeService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();

            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<ITokenRepository, TokenRepository>();
            builder.Services.AddScoped<ITokenGeneratorService, TokenGeneratorService>();
            builder.Services.AddScoped<IAbsencePredictorService, AbsencePredictorService>();

            // configure mail service
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<IEmailService, EmailService>();

            builder.Services.ConfigureAuthentication(builder.Configuration);
            builder.Services.AddAuthorization();
            //builder.Services.AddRouting();
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwagger();

            builder.Services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Absence Mgt. System Api v1"));

            // Apply migrations at startup
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AMSDbContext>();
                    SeedData.Seed(services).GetAwaiter(); // Call seeder class
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            app.Use(async (context, next) =>
            {
                Console.WriteLine($"Request Path: {context.Request.Path}");
                Console.WriteLine($"Authorization Header: {context.Request.Headers["Authorization"]}");
                await next.Invoke();
            });

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.MapControllers();
            app.UseCors("AllowOrigin");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run();
        }
    }
}
