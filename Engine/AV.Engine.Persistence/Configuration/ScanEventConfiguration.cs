using AV.Engine.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AV.Engine.Persistence.Configuration
{
    public class ScanEventConfiguration : IEntityTypeConfiguration<ScanEvent>
    {
        public void Configure(EntityTypeBuilder<ScanEvent> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Timestamp).IsRequired();

            builder.Property(e => e.EventType)
                .HasConversion<string>()
                .IsRequired();

            builder.HasMany(e => e.Threats)
                .WithOne(t => t.ScanEvent)
                .HasForeignKey(t => t.ScanEventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
