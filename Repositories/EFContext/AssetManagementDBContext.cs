using AssetManagementWebApi.Repositories.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AssetManagementWebApi.Repositories.EFContext;

public class AssetManagementDBContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public AssetManagementDBContext(DbContextOptions options) : base(options){}
    public DbSet<AppUser> AppUser { get; set; }
    public DbSet<AppRole> AppRole { get; set; }
    public DbSet<CategoryEntity> CategoryEntity { get; set; }
    public DbSet<AssetEntity> AssetEntity { get; set; }
    public DbSet<AssignmentEntity> AssignmentEntity { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("AppUserClaims");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("AppUserRoles").HasKey(x => new { x.UserId, x.RoleId });
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("AppUserLogins").HasKey(x => x.UserId);
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("AppRoleClaims");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("AppUserTokens").HasKey(x => x.UserId);

        modelBuilder.Entity<AssetEntity>().HasOne(x=>x.Category);
        modelBuilder.Entity<AssetEntity>().HasMany(x => x.HistoricalAssignments).WithOne(x =>x.Asset);
        
        
        modelBuilder.Entity<AppUser>().Property(c => c.IsFirstLogin).HasDefaultValue(true);
        modelBuilder.Entity<AppUser>().Property(c => c.IsDisabled).HasDefaultValue(false);
        modelBuilder.Entity<AppUser>().HasMany(c => c.HistoricalAssignments);
        
        modelBuilder.Entity<AssignmentEntity>().HasMany(x => x.RelatedUsers);
        AssetManagementSeedData.Seed(modelBuilder);
    }

}