using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Strawberry_Shortcake.Models;

namespace Strawberry_Shortcake.Data
{
    public class Strawberry_ShortcakeContext : DbContext
    {
        public Strawberry_ShortcakeContext (DbContextOptions<Strawberry_ShortcakeContext> options)
            : base(options)
        {
        }

        public DbSet<Strawberry_Shortcake.Models.User> User { get; set; }
    }
}
