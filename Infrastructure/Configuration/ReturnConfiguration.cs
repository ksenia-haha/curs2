
﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain;
using Domain.Entities;

namespace Infrastructure.Configuration
{
        public class ReturnConfiguration : IEntityTypeConfiguration<Return>
        {
            public void Configure(EntityTypeBuilder<Return> builder)
            {
                builder.HasKey(x => x.Id);
                builder.HasAlternateKey(x => x.Id);

            builder.HasOne(r => r.Employee)
            .WithMany(e => e.Returns);

            //builder.HasOne(r => r.Sale)
            //.WithMany(s => s.Returns);

            builder.HasOne(r => r.Client)
            .WithMany(c => c.Returns);

            builder.HasOne(r => r.Exemplar)
            .WithMany(e => e.Returns);
        }
        }
    }

