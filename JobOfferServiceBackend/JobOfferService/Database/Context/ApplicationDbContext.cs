using JobOfferService.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace JobOfferService.Database.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext() : base()
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>().HasKey(x => x.Id);
            modelBuilder.Entity<JobAdvertisement>().HasKey(x => x.Id);
            modelBuilder.Entity<CandidateApplication>().HasKey(x => x.Id);

            modelBuilder.Entity<JobAdvertisement>()
                .HasOne(x => x.Employer)
                .WithMany(x => x.JobAdvertisements)
                .HasForeignKey(x => x.EmployerId);

            modelBuilder.Entity<CandidateApplication>()
                .HasOne(x => x.JobAdvertisement)
                .WithMany(x => x.CandidateApplications)
                .HasForeignKey(x => x.JobAdvertisementId);

            //modelBuilder.Entity<UserPermission>()
            //        .HasOne(x => x.User)
            //        .WithMany(x => x.Permissions)
            //        .HasForeignKey(x => x.UserId);
        }

        public virtual DbSet<AppUser> Users { get; set; }
        public virtual DbSet<JobAdvertisement> JobAdvertisements { get; set; }
        public virtual DbSet<CandidateApplication> CandidateApplications { get; set; }

    }
}
