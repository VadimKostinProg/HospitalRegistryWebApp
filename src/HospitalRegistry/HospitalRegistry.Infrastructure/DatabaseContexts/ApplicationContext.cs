using HospitalReqistry.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HospitalRegistry.Infrastructure.DatabaseContexts
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        public ApplicationContext(DbContextOptions options) : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = true;
        }

        public ApplicationContext() { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TimeSlot>().ToTable("TimeSlots");
            modelBuilder.Entity<Doctor>().ToTable("Doctors");
            modelBuilder.Entity<Schedule>().ToTable("Schedules");
            modelBuilder.Entity<Patient>().ToTable("Patients");
            modelBuilder.Entity<Diagnosis>().ToTable("Diagnoses");
            modelBuilder.Entity<Appointment>().ToTable("Appointments");
        }
    }
}
