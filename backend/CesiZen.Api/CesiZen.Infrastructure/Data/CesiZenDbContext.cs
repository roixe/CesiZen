using CesiZen.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CesiZen.Infrastructure.Data
{
    public class CesiZenDbContext : DbContext
    {
        public CesiZenDbContext(DbContextOptions<CesiZenDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");

                entity.HasKey(u => u.Id);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(255);

                entity.HasIndex(u => u.Email)
                      .IsUnique();

                entity.Property(u => u.PasswordHash)
                      .IsRequired()
                      .HasMaxLength(255);
            });

        }
    }
}
