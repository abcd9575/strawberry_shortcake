using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Strawberry_Shortcake.Models;
using BC = BCrypt.Net.BCrypt;

namespace Strawberry_Shortcake.Data
{
    public class Strawberry_ShortcakeContext : DbContext
    {
        public Strawberry_ShortcakeContext()
        {
        }

        public Strawberry_ShortcakeContext (DbContextOptions<Strawberry_ShortcakeContext> options)
            : base(options)
        {
        }

        public DbSet<Strawberry_Shortcake.Models.User> Users { get; set; }
        public DbSet<Strawberry_Shortcake.Models.Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(r => r.UserEmail)
                .IsUnique();


            modelBuilder.Entity<Role>()
                .HasIndex(r => r.RoleName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasOne<Role>(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId);

            Seed(modelBuilder);
        }

        private void Seed(ModelBuilder modelBuilder) {
            Role[] 기본등급 = new Role[] {
                new Role{ Id = 1, RoleName = "User" },
                new Role{ Id = 2, RoleName = "Manager" },
                new Role{ Id = 3, RoleName = "Administrator" },
            };

            modelBuilder.Entity<Role>()
                .HasData(기본등급);

            modelBuilder.Entity<User>()
                .HasData(new User
                {
                    UserNo = 1,
                    UserEmail = "abcd95751@gmail.com",
                    UserName = string.Empty,
                    UserPw = BC.HashPassword("1q2w3e$R"),
                    Activation = true,
                    RoleId = 3
                });
            
        }
    }
}
