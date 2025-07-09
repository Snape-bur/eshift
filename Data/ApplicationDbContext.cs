using EShift.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EShift.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }
      
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Assistant> Assistants { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Job> Jobs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Job <-> Customer (AppUserId as FK to AppUserId in Customer)
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Customer)
                .WithMany(c => c.Jobs)
                .HasForeignKey(j => j.AppUserId)
                .HasPrincipalKey(c => c.AppUserId);

            // Optional: Decimal config for Vehicle (already exists)
            modelBuilder.Entity<Vehicle>()
                .Property(v => v.CapacityCubicFeet)
                .HasColumnType("decimal(18, 2)"); 

            // NEW: Job <-> Driver (One-to-Many, optional)
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Driver)
                .WithMany(d => d.AssignedJobs)
                .HasForeignKey(j => j.DriverId)
                .OnDelete(DeleteBehavior.SetNull);

            // NEW: Job <-> Vehicle (One-to-Many, optional)
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Vehicle)
                .WithMany(v => v.AssignedJobs)
                .HasForeignKey(j => j.VehicleId)
                .OnDelete(DeleteBehavior.SetNull);

            // NEW: Job <-> Assistant (Many-to-Many)
            modelBuilder.Entity<Job>()
                .HasMany(j => j.AssignedAssistants)
                .WithMany(a => a.AssignedJobs)
                .UsingEntity(j => j.ToTable("JobAssistants"));
        }

    }
}
