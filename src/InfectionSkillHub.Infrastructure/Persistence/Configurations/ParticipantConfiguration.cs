using InfectionSkillHub.Domain.Entities;
using InfectionSkillHub.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfectionSkillHub.Infrastructure.Persistence.Configurations;

public class ParticipantConfiguration : IEntityTypeConfiguration<Participant>
{
    public void Configure(EntityTypeBuilder<Participant> builder)
    {
        builder.ToTable("participants");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(p => p.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(p => p.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(Participant.FullNameMaxLength)
            .IsRequired();

        builder.Property(p => p.Email)
            .HasColumnName("email")
            .HasMaxLength(Participant.EmailMaxLength)
            .IsRequired();

        builder.Property(p => p.RegisteredAt)
            .HasColumnName("registered_at")
            .IsRequired();

        builder.HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<Participant>(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.Email).IsUnique();
        builder.HasIndex(p => p.UserId).IsUnique();
    }
}
