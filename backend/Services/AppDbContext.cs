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
        public DbSet<AppSettings> AppSettings { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Models.Action> Actions { get; set; }

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

            builder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();

            builder.Entity<Models.Action>()
                .HasIndex(a => a.UserId);

            builder.Entity<Models.Action>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "User" },
                new Role { Id = 2, Name = "Moderator" },
                new Role { Id = 3, Name = "Admin" }
            );

            builder.Entity<AppSettings>().HasData(new AppSettings
            {
                Id = 1,
            });
        }
    }
}
