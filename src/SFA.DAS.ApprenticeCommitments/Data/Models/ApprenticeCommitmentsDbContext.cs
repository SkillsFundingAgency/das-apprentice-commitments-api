using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Net.Mail;

namespace SFA.DAS.ApprenticeCommitments.Data.Models
{
    public class ApprenticeCommitmentsDbContext : DbContext, IRegistrationContext, IApprenticeContext, IApprenticeshipContext
    {
        public ApprenticeCommitmentsDbContext()
        {
        }

        public ApprenticeCommitmentsDbContext(DbContextOptions<ApprenticeCommitmentsDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Registration> Registrations { get; set; }
        public virtual DbSet<Apprentice> Apprentices { get; set; }
        public virtual DbSet<Apprenticeship> Apprenticeships { get; set; }

        DbSet<Registration> IEntityContext<Registration>.Entities => Registrations;
        DbSet<Apprentice> IEntityContext<Apprentice>.Entities => Apprentices;
        DbSet<Apprenticeship> IEntityContext<Apprenticeship>.Entities => Apprenticeships;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Apprentice>(a =>
            {
                a.ToTable("Apprentice");
                a.HasKey(e => e.Id);
                a.Property(e => e.Email)
                 .HasConversion(
                    v => v.ToString(),
                    v => new MailAddress(v));
                a.OwnsMany(
                    e => e.PreviousEmailAddresses,
                    c =>
                    {
                        c.HasKey("Id");
                        c.Property(typeof(long), "Id");
                        c.HasIndex("ApprenticeId");
                        c.Property(e => e.EmailAddress)
                            .HasConversion(
                                v => v.ToString(),
                                v => new MailAddress(v));
                    });
                a.HasMany(e => e.Apprenticeships).WithOne(a => a.Apprentice);
                a.Property(e => e.CreatedOn).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<Apprenticeship>(a =>
            {
                a.Property(typeof(long), "Id");
                a.HasKey("Id");
                a.Property(e => e.ApprenticeshipId).HasDefaultValue(0);
            });

            modelBuilder.Entity<Apprenticeship>()
                .OwnsOne(e => e.Details, details =>
                {
                    details.Property(p => p.EmployerAccountLegalEntityId).HasColumnName("EmployerAccountLegalEntityId");
                    details.Property(p => p.EmployerName).HasColumnName("EmployerName");
                    details.Property(p => p.TrainingProviderId).HasColumnName("TrainingProviderId");
                    details.Property(p => p.TrainingProviderName).HasColumnName("TrainingProviderName");
                    details.OwnsOne(e => e.Course, course =>
                    {
                        course.Property(p => p.Name).HasColumnName("CourseName");
                        course.Property(p => p.Level).HasColumnName("CourseLevel");
                        course.Property(p => p.Option).HasColumnName("CourseOption");
                        course.Property(p => p.PlannedStartDate).HasColumnName("PlannedStartDate");
                        course.Property(p => p.PlannedEndDate).HasColumnName("PlannedEndDate");
                    });
                });

            modelBuilder.Entity<Registration>(entity =>
            {
                entity.HasKey(e => e.ApprenticeId);

                entity.Property(e => e.CreatedOn).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                entity.Property(e => e.Email)
                    .HasConversion(
                        v => v.ToString(),
                        v => new MailAddress(v));
            });

            modelBuilder.Entity<Registration>(entity =>
            {
                entity.Property(e => e.CreatedOn).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
                entity.OwnsOne(e => e.Apprenticeship, apprenticeship =>
                {
                    apprenticeship.Property(p => p.EmployerAccountLegalEntityId)
                        .HasColumnName("EmployerAccountLegalEntityId");
                    apprenticeship.Property(p => p.EmployerName).HasColumnName("EmployerName");
                    apprenticeship.Property(p => p.TrainingProviderId).HasColumnName("TrainingProviderId");
                    apprenticeship.Property(p => p.TrainingProviderName).HasColumnName("TrainingProviderName");
                    apprenticeship.OwnsOne(e => e.Course, course =>
                    {
                        course.Property(p => p.Name).HasColumnName("CourseName");
                        course.Property(p => p.Level).HasColumnName("CourseLevel");
                        course.Property(p => p.Option).HasColumnName("CourseOption");
                        course.Property(p => p.PlannedStartDate).HasColumnName("PlannedStartDate");
                        course.Property(p => p.PlannedEndDate).HasColumnName("PlannedEndDate");
                    });
                });
            });
            base.OnModelCreating(modelBuilder);
        }
    }
}