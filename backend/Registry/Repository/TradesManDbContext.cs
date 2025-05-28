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
        public virtual DbSet<Speciality> Specialties { get; set; }
        public virtual DbSet<TradesManSpecialities> TradesManSpecialities { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>()
                   .HasOne(x => x.Client)
                   .WithMany().OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>()
                .HasOne(x => x.TradesManProfile)
                .WithOne().OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey<TradesMan>(x => x.Id);

            modelBuilder.Entity<Speciality>().HasIndex(e => e.Type).IsUnique();

            modelBuilder.Entity<TradesManSpecialities>()
                .HasKey(e => new { e.TradesManId, e.SpecialityId });

            modelBuilder.Entity<TradesManSpecialities>()
                .HasOne(e => e.TradesMan)
                .WithMany(s => s.Specialities)
                .HasForeignKey(e => e.TradesManId);

            modelBuilder.Entity<TradesManSpecialities>()
                .HasOne(e => e.Speciality)
                .WithMany(c => c.TradesMenSpecialities)
                .HasForeignKey(e => e.SpecialityId);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
