using Master_DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Master_DAL.DbContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            //var databaseCreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
            //if(databaseCreator != null)
            //{
            //    if (!databaseCreator.CanConnect()) databaseCreator.Create();
            //    if(!databaseCreator.HasTables()) databaseCreator.CreateTables();

            //}
        }


        protected override void OnModelCreating(ModelBuilder builder)
        {

            #region Articles and ArticlesImages(1:m)
            builder.Entity<Articles>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(e => e.Title).IsRequired();
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.PublishedDate).IsRequired();
                entity.Property(e => e.IsActive).IsRequired(false);

                entity.HasMany(x=>x.ArticlesImages)
                .WithOne(x=>x.Articles)
                .HasForeignKey(x=>x.ArticlesId)
                .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region ArticlesImage and Articles(m:1)
            builder.Entity<ArticlesImage>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(e=>e.ImagesUrl).IsRequired();

                entity.HasOne(x=>x.Articles)
                .WithMany(x=>x.ArticlesImages)
                .HasForeignKey(x=>x.ArticlesId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            #endregion

            #region ApplicationUser and Articles(1:m)
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.Property(e=>e.FirstName).IsRequired(false);
                entity.Property(e=>e.LastName).IsRequired(false);
                entity.Property(e=>e.Address).IsRequired(false);

                entity.HasMany(a => a.Articles)
                .WithOne(a => a.ApplicationUsers)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);
                
            });

            #endregion

            #region Articles and ApplicationUser(m:1)
            builder.Entity<Articles>(entity =>
            {
                entity.HasOne(a=>a.ApplicationUsers)
                .WithMany(a=>a.Articles)
                .HasForeignKey(a=>a.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            });

            #endregion

            #region Articles and Comments(1:m)
            builder.Entity<Articles>(entity =>
            {
                entity.HasMany(x => x.Comments)
                .WithOne(x => x.Articles)
                .HasForeignKey(x => x.ArticlesId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            #endregion

            #region Commens and Articles(m:1)
            builder.Entity<Comments>(entity =>
            {
                entity.HasKey(u=> u.Id);
                entity.Property(e=>e.Content).IsRequired();
                entity.HasOne(x=>x.Articles)
                .WithMany(x=>x.Comments)
                .HasForeignKey(x=>x.ArticlesId)
                .OnDelete(DeleteBehavior.Cascade);
            });

            #endregion

            #region Comments and ApplicationUser(m:1)
            builder.Entity<Comments>(entity =>
            {
                entity.HasOne(x => x.ApplicationUsers)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            });

            #endregion

            #region ApplicationUSer and Comments(1:m)
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasMany(x=>x.Comments)
                .WithOne(x=>x.ApplicationUsers)
                .HasForeignKey(x=>x.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            });

            #endregion

            #region Like and ApplicationUser(m:1)
            builder.Entity<Likes>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x=>x.User)
                .WithMany(x=>x.Likes)
                .HasForeignKey(x=>x.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            });
            #endregion

            #region ApplicationUser and Like(1:m)
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.HasMany(x=>x.Likes)
                .WithOne(x=>x.User)
                .HasForeignKey(x=>x.UserId)
                .OnDelete(DeleteBehavior.NoAction);
            });

            #endregion

            #region Use Polymorphic Association
            builder.Entity<Articles>(entity =>
            {
                entity.HasMany(e => e.Likes)
                .WithOne()
                .HasForeignKey(e => e.LikelableId)
                .HasPrincipalKey(e => e.Id)
                .OnDelete(DeleteBehavior.NoAction);

            });

            builder.Entity<Comments>(entity =>
            {
                entity.HasMany(e => e.Likes)
                .WithOne()
                .HasForeignKey(e => e.Id)
                .HasPrincipalKey(e => e.Id)
                .OnDelete(DeleteBehavior.NoAction);

            });


            #endregion

            #region User and Permission(m:m)
            builder.Entity<UserPermission>()
            .HasKey(up => new { up.UserId, up.PermissionId });

            builder.Entity<UserPermission>()
            .HasOne(up => up.User)
            .WithMany(u => u.UserPermissions)
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserPermission>()
            .HasOne(up => up.Permissions)
            .WithMany(p => p.UserPermissions)
            .HasForeignKey(up => up.PermissionId);

            #endregion

            #region Permission and ControllerAction(m:m)

            builder.Entity<PermissionControllerAction>()
            .HasKey(pca => new { pca.PermissionId, pca.ControlleractionId });

            builder.Entity<PermissionControllerAction>()
                .HasOne(pca => pca.Permission)
                .WithMany(p => p.PermissionControllerActions)
                .HasForeignKey(pca => pca.PermissionId);

            builder.Entity<PermissionControllerAction>()
                .HasOne(pca => pca.ControllerAction)
                .WithMany(ca => ca.PermissionControllerActions)
                .HasForeignKey(pca => pca.ControlleractionId);

            #endregion


            #region Menu and SubModule(m:1)
            builder.Entity<Menu>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.SubModules)
                .WithMany(x => x.Menu)
                .HasForeignKey(x => x.SubModuleId)
                .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion

            #region SubModule and Menu(1:m)
            builder.Entity<SubModules>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasMany(x => x.Menu)
                .WithOne(x => x.SubModules)
                .HasForeignKey(x => x.SubModuleId)
                .OnDelete(DeleteBehavior.Cascade);
            });
            #endregion


            #region Module and SubModule(1:m)
            builder.Entity<Modules>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasMany(x => x.SubModules)
                .WithOne(x => x.Modules)
                .HasForeignKey(x => x.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            });

            #endregion

            #region SubModule and Module(m:1)
            builder.Entity<SubModules>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasOne(x => x.Modules)
                .WithMany(x => x.SubModules)
                .HasForeignKey(x => x.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            });
            #endregion


            #region Modules and Roles(m:m)

            // Configure many-to-many relationship between roles and modules
            builder.Entity<RoleModule>()
                .HasKey(rm => new { rm.RoleId, rm.ModuleId });

            builder.Entity<RoleModule>()
                .HasOne(rm => rm.Role)
                .WithMany()
                .HasForeignKey(rm => rm.RoleId);

            builder.Entity<RoleModule>()
                .HasOne(rm => rm.Modules)
                .WithMany(m => m.RoleModules)
                .HasForeignKey(rm => rm.ModuleId);

            #endregion



            base.OnModelCreating(builder);
        }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Articles> Articles { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<ArticlesImage> ArticlesImages { get; set; }

        #region Permission, ControllerAction, PermissionControllerAction
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<ControllerAction> ControllerActions { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<PermissionControllerAction> PermissionControllerActions { get; set; }

        #endregion


        #region Menu,Module,SubModule
        public DbSet<Modules> Modules { get; set; }
        public DbSet<SubModules> SubModules { get; set; }
        public DbSet<Menu> Menus { get; set; }

        public DbSet<RoleModule> RoleModules { get; set; }

        #endregion
    }
}
