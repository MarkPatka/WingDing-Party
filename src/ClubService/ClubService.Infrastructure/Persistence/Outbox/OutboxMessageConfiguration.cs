using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClubService.Infrastructure.Persistence.Outbox
{
    public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.ToTable("OutboxMessages");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .IsRequired();

            builder.Property(x => x.Type)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(x => x.Payload)
                .IsRequired();

            builder.Property(x => x.OccurredOnUtc)
                .IsRequired();

            builder.Property(x => x.ProcessedOnUtc);

            builder.Property(x => x.Error);

            builder.Property(x => x.Retries)
                .IsRequired()
                .HasDefaultValue(0);

            // Индекс на необработанные события для быстрого чтения
            builder.HasIndex(x => new { x.ProcessedOnUtc, x.OccurredOnUtc });
        }
    }
}