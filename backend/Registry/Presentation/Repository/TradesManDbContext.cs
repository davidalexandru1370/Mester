using Microsoft.EntityFrameworkCore;
using Registry.Models;

namespace Registry.Repository
{
    public record ConnectionString(string Connection);
    public class TradesManDbContext : DbContext
    {
        private readonly string _connectionString;

        public TradesManDbContext(ConnectionString connectionString)
        {
            _connectionString = connectionString.Connection;
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<TradesMan> TradesMen { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Post>()
            //    .HasOne(p => p.Blog)
            //    .WithMany(b => b.Posts)
            //    .HasForeignKey(p => p.BlogUrl)
            //    .HasPrincipalKey(b => b.Url);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
