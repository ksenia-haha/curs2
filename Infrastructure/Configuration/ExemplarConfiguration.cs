
﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain;

namespace Infrastructure.Configuration
{
    public class ExemplarConfiguration : IEntityTypeConfiguration<Exemplar>
    {
        public void Configure(EntityTypeBuilder<Exemplar> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(e => e.Edition)
                .WithMany(e => e.Exemplars);
        }
    }
}
