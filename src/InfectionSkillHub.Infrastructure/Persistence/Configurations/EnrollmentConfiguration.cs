using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Domain.Enums;
using InfectionSkillHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfectionSkillHub.Infrastructure.Persistence.Configurations;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("enrollments", table =>
        {
            // A cancellation always records when it happened and who did it.
            table.HasCheckConstraint(
                "ck_enrollments_cancellation",
                "(status = 'Active' AND cancelled_at IS NULL) OR (status = 'Cancelled' AND cancelled_at IS NOT NULL AND cancelled_by_email IS NOT NULL)");
        });

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(e => e.WorkshopId)
            .HasColumnName("workshop_id")
            .IsRequired();

        builder.Property(e => e.ParticipantId)
            .HasColumnName("participant_id")
            .IsRequired();

        // Stored as text so the database is readable without the enum.
        builder.Property(e => e.Status)
            .HasColumnName("status")
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.EnrolledAt)
            .HasColumnName("enrolled_at")
            .IsRequired();

        builder.Property(e => e.CancelledAt)
            .HasColumnName("cancelled_at");

        builder.Property(e => e.CancelledByUserId)
            .HasColumnName("cancelled_by_user_id");

        builder.Property(e => e.CancelledByEmail)
            .HasColumnName("cancelled_by_email")
            .HasMaxLength(256);

        builder.HasOne(e => e.Workshop)
            .WithMany(w => w.Enrollments)
            .HasForeignKey(e => e.WorkshopId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Participant)
            .WithMany(p => p.Enrollments)
            .HasForeignKey(e => e.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.CancelledByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Last line of defense against duplicates: only one active enrollment per participant and workshop.
        // Cancelled ones stay as history, so the participant can enroll again.
        builder.HasIndex(e => new { e.ParticipantId, e.WorkshopId })
            .IsUnique()
            .HasFilter($"status = '{nameof(EnrollmentStatus.Active)}'")
            .HasDatabaseName("ux_enrollments_active_participant_workshop");

        builder.HasIndex(e => new { e.WorkshopId, e.Status });
    }
}
