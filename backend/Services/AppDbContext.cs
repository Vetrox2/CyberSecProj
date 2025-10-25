using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<OneTimePassword> OneTimePasswords { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasIndex(u => u.Login)
                .IsUnique();

            builder.Entity<User>()
                .Property(u => u.Login)
                .IsRequired()
                .HasMaxLength(100);

            builder.Entity<User>()
                .Property(u => u.PasswordHash)
                .IsRequired();

            builder.Entity<AuditLog>()
                .HasIndex(a => a.UserId);

            builder.Entity<AuditLog>()
                .HasIndex(a => a.Date);

            builder.Entity<AuditLog>()
                .HasIndex(a => a.ActionType);

            builder.Entity<OneTimePassword>()
                .HasIndex(otp => otp.UserLogin);

            builder.Entity<OneTimePassword>()
                .HasIndex(otp => new { otp.UserLogin, otp.Active });
        }
    }
}
