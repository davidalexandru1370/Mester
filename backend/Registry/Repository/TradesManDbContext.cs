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
        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Speciality> Specialties { get; set; }
        public virtual DbSet<TradesManSpecialities> TradesManSpecialities { get; set; }

        public virtual DbSet<ClientJobRequest> ClientRequests { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Conversation> Conversations { get; set; }
        public virtual DbSet<TradesManJobResponse> TradesManJobResponses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>()
                .HasOne(x => x.TradesManJobResponse)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(x => x.TradesManProfile)
                .WithOne().OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey<TradesMan>(x => x.Id);

            modelBuilder.Entity<Speciality>().HasIndex(e => e.Type).IsUnique();

            modelBuilder.Entity<TradesManJobResponse>().Property(x => x.WorkmanshipAmount).HasPrecision(18, 2);

            modelBuilder.Entity<Conversation>().HasOne(x => x.Request).WithMany().OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Conversation>().HasOne(x => x.TradesMan).WithMany().OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Conversation>().HasIndex(c => new { c.RequestId, c.TradesManId }).IsUnique();

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

            modelBuilder.Entity<ClientJobRequest>().HasOne(x => x.JobApproved).WithOne();
            modelBuilder.Entity<ClientJobRequest>().HasOne(x => x.InitiatedBy).WithMany().OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Bill>().Property(b => b.Amount).HasPrecision(18, 2);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
