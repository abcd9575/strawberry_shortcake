﻿using System;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(r => r.UserEmail)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion(
                    r => r.ToString(), // DB에 저장할 때 String으로 저장
                    r => Enum.Parse<Role>(r) // DB에서 가져올 때 Role 타입으로 가져옴
                );

            Seed(modelBuilder);
        }

        private void Seed(ModelBuilder modelBuilder) {

            modelBuilder.Entity<User>()
                .HasData(new User
                {
                    UserNo = 1,
                    UserEmail = "abcd95751@gmail.com",
                    UserName = string.Empty,
                    UserPw = BC.HashPassword("1q2w3e$R"),
                    Activation = true,
                    Role = Role.Administrator
                });
            
        }
    }
}
