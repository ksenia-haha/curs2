
﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain;

namespace Infrastructure.Configuration
{
        public class ClientConfiguration : IEntityTypeConfiguration<Client>
        {
            public void Configure(EntityTypeBuilder<Client> builder)
            {
                builder.HasKey(x => x.Id);
                builder.HasAlternateKey(x => x.Id);

            builder.HasMany(c => c.Sales)
            .WithOne(s => s.Client);
            }
        }
    }

