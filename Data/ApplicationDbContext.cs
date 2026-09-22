using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GiftOfTheGivers.Models;

namespace GiftOfTheGivers.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // public DbSet<Employee> Employees { get; set; }
        public DbSet<ReliefProject> ReliefProjects { get; set; }
        public DbSet<Donation> Donations { get; set; }
        // public DbSet<DonationSchedule> DonationSchedules { get; set; }
        public DbSet<TaxCertificate> TaxCertificates { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<ProjectVolunteer> ProjectVolunteers { get; set; }
        // public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // MUST be called first

            // 1-to-1: User <-> Employee
            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.UserID)
                .IsUnique();

            // 1-to-1: User <-> Volunteer
            modelBuilder.Entity<Volunteer>()
                .HasIndex(v => v.UserID)
                .IsUnique();

            // 1-to-1: Donation <-> TaxCertificate
            modelBuilder.Entity<TaxCertificate>()
                .HasIndex(t => t.DonationID)
                .IsUnique();

            // Composite Unique Constraint: ProjectVolunteers
            modelBuilder.Entity<ProjectVolunteer>()
                .HasIndex(pv => new { pv.ProjectID, pv.VolunteerID })
                .IsUnique();

            // Performance Indexes
            modelBuilder.Entity<Donation>()
                .HasIndex(d => new { d.UserID, d.DonationDate })
                .HasDatabaseName("IX_Donations_UserID_DonationDate");

            modelBuilder.Entity<DonationSchedule>()
                .HasKey(ds => ds.DonationScheduleID);

            modelBuilder.Entity<ProjectVolunteer>()
                .HasIndex(pv => new { pv.ProjectID, pv.AssignmentStatus })
                .HasDatabaseName("IX_ProjectVolunteers_ProjectID_Status");

            modelBuilder.Entity<AuditLog>()
                .HasIndex(a => new { a.EntityName, a.EntityID, a.CreatedDate })
                .HasDatabaseName("IX_AuditLogs_EntityName_EntityID_CreatedDate");
        }
    }
}