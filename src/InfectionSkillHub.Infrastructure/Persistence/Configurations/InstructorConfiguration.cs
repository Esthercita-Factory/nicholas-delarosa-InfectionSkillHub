using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfectionSkillHub.Infrastructure.Persistence.Configurations;

public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.ToTable("instructors");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(i => i.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(i => i.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(Instructor.FullNameMaxLength)
            .IsRequired();

        builder.Property(i => i.Email)
            .HasColumnName("email")
            .HasMaxLength(Instructor.EmailMaxLength)
            .IsRequired();

        builder.Property(i => i.Specialty)
            .HasColumnName("specialty")
            .HasMaxLength(Instructor.SpecialtyMaxLength)
            .IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<Instructor>(i => i.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(i => i.Email).IsUnique();
        builder.HasIndex(i => i.UserId).IsUnique();
    }
}
