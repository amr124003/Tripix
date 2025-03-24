using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tripix.Entities;

namespace Tripix.Context
{
    public class ApplicationDbcontext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbcontext ()
        {

        }

        public ApplicationDbcontext ( DbContextOptions<ApplicationDbcontext> options ) : base(options)
        {

        }

        protected override void OnModelCreating ( ModelBuilder modelBuilder )
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbcontext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Question> Questions { get; set; }
        public DbSet<CreditCard> CreditCards { get; set; }

    }
}
