using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telemetry.Entities.Models;

namespace Telemetry.Entities.Configurations
{
    public class TelemetrySessionConfiguration : IEntityTypeConfiguration<TelemetrySession>
    {
        public void Configure(EntityTypeBuilder<TelemetrySession> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.HasMany(x => x.Pages)
                .WithOne(x => x.TelemetrySession)
                .HasForeignKey(x => x.TelemetrySessionId);
        }
    }
}
