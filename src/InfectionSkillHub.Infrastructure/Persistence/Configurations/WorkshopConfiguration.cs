using InfectionSkillHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfectionSkillHub.Infrastructure.Persistence.Configurations;

public class WorkshopConfiguration : IEntityTypeConfiguration<Workshop>
{
    public void Configure(EntityTypeBuilder<Workshop> builder)
    {
        builder.ToTable("workshops", table =>
        {
            table.HasCheckConstraint("ck_workshops_schedule", "ends_at > starts_at");
            table.HasCheckConstraint("ck_workshops_capacity", "capacity > 0");
        });

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(w => w.Code)
            .HasColumnName("code")
            .HasMaxLength(Workshop.CodeMaxLength)
            .IsRequired();

        builder.Property(w => w.Title)
            .HasColumnName("title")
            .HasMaxLength(Workshop.TitleMaxLength)
            .IsRequired();

        builder.Property(w => w.Description)
            .HasColumnName("description")
            .HasMaxLength(Workshop.DescriptionMaxLength)
            .IsRequired();

        builder.Property(w => w.InstructorId)
            .HasColumnName("instructor_id")
            .IsRequired();

        builder.Property(w => w.StartsAt)
            .HasColumnName("starts_at")
            .IsRequired();

        builder.Property(w => w.EndsAt)
            .HasColumnName("ends_at")
            .IsRequired();

        builder.Property(w => w.Capacity)
            .HasColumnName("capacity")
            .IsRequired();

        builder.Property(w => w.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        // An instructor with workshops cannot be deleted.
        builder.HasOne(w => w.Instructor)
            .WithMany(i => i.Workshops)
            .HasForeignKey(w => w.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(w => w.Code).IsUnique();
        builder.HasIndex(w => w.StartsAt);
        builder.HasIndex(w => w.InstructorId);
    }
}
