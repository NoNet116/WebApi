using DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(Microsoft.EntityFrameworkCore.DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(e =>
            {
                // UserName
                e.Property(u => u.UserName)
                    .IsRequired()
                    .HasMaxLength(50);

                e.HasIndex(u => u.UserName)
                    .IsUnique()
                    .HasDatabaseName("IX_User_UserName");

                // Email
                e.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                e.HasIndex(u => u.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_User_Email");
            });
        }   }
}
