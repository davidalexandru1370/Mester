using Microsoft.EntityFrameworkCore;
using Registry.Models;

namespace Registry.Repository
{
    public record ConnectionString(string Connection);
    public class TradesManDbContext : DbContext
    {
        private readonly string _connectionString;

        public TradesManDbContext(DbContextOptions<TradesManDbContext> options, ConnectionString connectionString) : base(options)
        {
            _connectionString = connectionString.Connection;
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Specialty> Specialties { get; set; }

        public virtual DbSet<ClientRequest> ClientRequests { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Conversation> Conversations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>()
                   .HasOne(x => x.Client)
                   .WithMany().OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>()
                .HasOne(x => x.TradesManProfile)
                .WithOne().OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey<TradesMan>(x => x.Id);

            modelBuilder.Entity<Specialty>().HasIndex(e => e.Type).IsUnique();

            modelBuilder.Entity<ClientRequest>().Property(x => x.WorkmanshipAmount).HasPrecision(18, 2);
            modelBuilder.Entity<ClientRequest>().HasOne(x => x.To).WithMany().OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Conversation>().HasOne(x => x.User1).WithMany().OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Conversation>().HasOne(x => x.User2).WithMany().OnDelete(DeleteBehavior.Restrict);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
