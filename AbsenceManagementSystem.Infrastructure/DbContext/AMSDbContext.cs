using AbsenceManagementSystem.Core.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AbsenceManagementSystem.Infrastructure.DbContext
{
    public class AMSDbContext : IdentityDbContext
    {
        public AMSDbContext(DbContextOptions<AMSDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
    }
}
