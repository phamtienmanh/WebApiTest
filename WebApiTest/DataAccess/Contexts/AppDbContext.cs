using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApiTest.DataAccess.Entities;

namespace WebApiTest.DataAccess.Contexts
{
    public sealed class AppDbContext: IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostTranslation> PostTranslations { get; set; }
        public DbSet<PostMeta> PostMetas { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
