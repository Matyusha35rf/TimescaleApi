using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configurations
{
    public class ResultConfiguration : IEntityTypeConfiguration<Result>
    {
        public void Configure(EntityTypeBuilder<Result> builder)
        {
            // Первичный ключ
            builder.HasKey(x => x.Id);

            // FileName должен быть уникальным
            builder.HasIndex(x => x.FileName)
                .IsUnique()
                .HasDatabaseName("IX_Results_FileName");

            // Индексы для фильтрации
            builder.HasIndex(x => x.FirstOperationDate)
                .HasDatabaseName("IX_Results_FirstOperationDate");

            builder.HasIndex(x => x.AverageValue)
                .HasDatabaseName("IX_Results_AverageValue");

            builder.HasIndex(x => x.AverageExecutionTime)
                .HasDatabaseName("IX_Results_AverageExecutionTime");
        }
    }
}