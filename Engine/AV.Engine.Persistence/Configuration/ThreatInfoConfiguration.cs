using AV.Engine.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AV.Engine.Persistence.Configuration
{
    public class ThreatInfoConfiguration : IEntityTypeConfiguration<ThreatInfo>
    {
        public void Configure(EntityTypeBuilder<ThreatInfo> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.FilePath).IsRequired();

            builder.Property(t => t.FilePath).IsRequired();

            builder.Property(t => t.ThreatName).IsRequired();
        }
    }
}
