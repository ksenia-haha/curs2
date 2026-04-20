
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    public class SaleAndExemplarConfiguration : IEntityTypeConfiguration<SaleAndExemplar>
    {
        public void Configure(EntityTypeBuilder<SaleAndExemplar> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Exemplar)
                .WithMany(e => e.salesAndExemplars);

            builder.HasOne(x => x.Sale)
                .WithMany(s => s.salesAndExemplars);
        }
    }
}
