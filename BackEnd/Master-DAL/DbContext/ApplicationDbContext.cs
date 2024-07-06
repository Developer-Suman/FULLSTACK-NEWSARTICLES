using Master_DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            if(databaseCreator != null)
            {
                if (!databaseCreator.CanConnect()) databaseCreator.Create();
                if(!databaseCreator.HasTables()) databaseCreator.CreateTables();

            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArticlesImage>()
                .HasKey(ai => ai.ArticlesImageId);
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Articles> Articles { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<ArticlesImage> ArticlesImages { get; set; }
    }
}
