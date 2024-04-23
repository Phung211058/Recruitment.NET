using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Job22.Models;

namespace Job22.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Job22.Models.Employer> Employer { get; set; } = default!;
        public DbSet<Job22.Models.Candidate> Candidate { get; set; } = default!;
        public DbSet<Job22.Models.JobList> JobList { get; set; } = default!;
        public DbSet<Job22.Models.Application> Application { get; set; } = default!;
    }
}
