using System;
using System.Collections.Generic;
using System.Text;
using DataAccess.Configurations;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<ValueRecord> Values { get; set; }
        public DbSet<Result> Results { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ResultConfiguration());
            modelBuilder.ApplyConfiguration(new ValueRecordConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
