using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
namespace EnglishApp.Data
{
    public class EnglishAppDbContext : IdentityDbContext<
        User,
        Role,
        int,
        UserClaim,
        UserRole,
        UserLogin,
        RoleClaim,
        UserToken
        >
    {
        public EnglishAppDbContext(DbContextOptions<EnglishAppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles {  get; set; }

        public DbSet<UserClaim> UserClaims { get; set; }
        public DbSet<UserLogin> UserLogins {  get; set; }
        public DbSet<UserToken> UserTokens {get;set; }
        public DbSet<RoleClaim> RoleClaims {get; set; }

        protected override void OnModelCreating(ModelBuilder builder) 
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().ToTable("users");
            builder.Entity<Role>().ToTable("roles");
            builder.Entity<UserRole>().ToTable("userroles");
            builder.Entity<UserClaim>().ToTable("userclaims");
            builder.Entity<UserLogin>().ToTable("userlogins");
            builder.Entity<UserToken>().ToTable("usertokens");
            builder.Entity<RoleClaim>().ToTable("roleclaims");
            
            builder.Entity<UserRole>()
                .HasKey(x=> new {x.UserId, x.RoleId});
            builder.Entity<UserRole>()
                .HasOne(x=>x.User)
                .WithMany(a=>a.UserRoles)
                .HasForeignKey(x=>x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserRole>()
                .HasOne(x=>x.Role)
                .WithMany(a=>a.UserRoles)
                .HasForeignKey(x=>x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            


        }
    }
}
