using Microsoft.EntityFrameworkCore;
using PX1LZ1_HSZF_2024251.Model;

namespace PX1LZ1_HSZF_2024251.Persistence.MsSql
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<World> Worlds { get; set; }
        public DbSet<Damage> Damages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<World>()
                .HasMany(w => w.Damages)
                .WithOne(d => d.World)
                .HasForeignKey(d => d.WorldId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Damage>()
                .Property(d => d.InvolvedParties)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
        }
    }
}