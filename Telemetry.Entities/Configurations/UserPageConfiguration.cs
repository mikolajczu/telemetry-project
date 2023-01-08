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
    public class UserPageConfiguration : IEntityTypeConfiguration<UserPage>
    {
        public void Configure(EntityTypeBuilder<UserPage> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
        }
    }
}
