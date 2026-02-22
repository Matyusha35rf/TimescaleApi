using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Configurations
{
    public class ValueRecordConfiguration : IEntityTypeConfiguration<ValueRecord>
    {
        public void Configure(EntityTypeBuilder<ValueRecord> builder) 
        {
            // Первичный ключ
            builder.HasKey(x => x.Id);

            // Индексы
            builder.HasIndex(x => new { x.FileName, x.Date })
                .HasDatabaseName("IX_Values_FileName_Date");

            builder.HasIndex(x => x.FileName)
                .HasDatabaseName("IX_Values_FileName");

            // Связь с Result
            builder.HasOne(v => v.Result)
                .WithMany(r => r.ValueRecords)
                .HasPrincipalKey(r => r.FileName)
                .HasForeignKey(v => v.FileName)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
